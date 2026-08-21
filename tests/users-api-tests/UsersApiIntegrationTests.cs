using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Prometheus.Users.Api.DTOs;
using Prometheus.Users.Api.Models;
using Xunit;

namespace Prometheus.Users.Api.Tests;

public sealed class UsersApiIntegrationTests : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();
    private readonly UsersApiFactory _factory = new();
    private readonly HttpClient _client;

    public UsersApiIntegrationTests()
    {
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidAdminCredentials_ReturnsJwt()
    {
        var response = await LoginAsync("admin@prometheus.local", "Admin@123");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var login = await ReadAsync<LoginResponse>(response);
        Assert.False(string.IsNullOrWhiteSpace(login.Token));
        Assert.True(login.ExpiresAt > DateTimeOffset.UtcNow);
        Assert.Equal(UserRole.Admin, login.User.Role);
        Assert.Equal("admin@prometheus.local", login.User.Email);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var response = await LoginAsync("admin@prometheus.local", "senha-incorreta");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Credenciais inválidas.", payload);
    }

    [Fact]
    public async Task Me_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_WithValidToken_ReturnsAuthenticatedUser()
    {
        await AuthenticateAsAdminAsync();

        var response = await _client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await ReadAsync<AuthenticatedUserResponse>(response);
        Assert.Equal("Administrador", user.Name);
        Assert.Equal("admin@prometheus.local", user.Email);
        Assert.Equal(UserRole.Admin, user.Role);

        var demoResponse = await _client.GetAsync("/api/demo/authenticated");
        Assert.Equal(HttpStatusCode.OK, demoResponse.StatusCode);
    }

    [Fact]
    public async Task Admin_CanCreateAluno()
    {
        await AuthenticateAsAdminAsync();

        var response = await CreateUserAsync(
            "Alice Aluno",
            "alice.create@prometheus.local",
            "Senha@123",
            "Aluno");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var user = await ReadAsync<UserResponse>(response);
        Assert.Equal(UserRole.Aluno, user.Role);
        Assert.Equal("alice.create@prometheus.local", user.Email);
        Assert.DoesNotContain("password", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Admin_CanCreateProfessor()
    {
        await AuthenticateAsAdminAsync();

        var response = await CreateUserAsync(
            "Paulo Professor",
            "paulo.create@prometheus.local",
            "Senha@123",
            "Professor");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var user = await ReadAsync<UserResponse>(response);
        Assert.Equal(UserRole.Professor, user.Role);

        var listResponse = await _client.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var users = await ReadAsync<List<UserResponse>>(listResponse);
        Assert.Contains(users, item => item.Email == "paulo.create@prometheus.local");
        Assert.DoesNotContain(
            "passwordHash",
            await listResponse.Content.ReadAsStringAsync(),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Admin_CannotCreateAnotherAdmin()
    {
        await AuthenticateAsAdminAsync();

        var response = await CreateUserAsync(
            "Outro Admin",
            "outro.admin@prometheus.local",
            "Senha@123",
            "Admin");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateEmail_ReturnsConflict()
    {
        await AuthenticateAsAdminAsync();
        var first = await CreateUserAsync(
            "Primeiro Aluno",
            "duplicado@prometheus.local",
            "Senha@123",
            "Aluno");
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var duplicate = await CreateUserAsync(
            "Segundo Aluno",
            "DUPLICADO@prometheus.local",
            "Senha@456",
            "Aluno");

        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
    }

    [Fact]
    public async Task Aluno_CannotCreateUser()
    {
        await CreateAndAuthenticateAsync(
            "Alice Aluno",
            "alice.no-create@prometheus.local",
            "Aluno");

        var response = await CreateUserAsync(
            "Outro Aluno",
            "outro@prometheus.local",
            "Senha@123",
            "Aluno");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Professor_CannotCreateUser()
    {
        await CreateAndAuthenticateAsync(
            "Paulo Professor",
            "paulo.no-create@prometheus.local",
            "Professor");

        var response = await CreateUserAsync(
            "Novo Aluno",
            "novo@prometheus.local",
            "Senha@123",
            "Aluno");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Aluno_CannotAccessAdminEndpoint()
    {
        await CreateAndAuthenticateAsync(
            "Alice Aluno",
            "alice.no-admin@prometheus.local",
            "Aluno");

        var response = await _client.GetAsync("/api/demo/admin");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Aluno_CannotAccessProfessorEndpoint()
    {
        await CreateAndAuthenticateAsync(
            "Alice Aluno",
            "alice.no-professor@prometheus.local",
            "Aluno");

        var response = await _client.GetAsync("/api/demo/professor");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Professor_CannotAccessAdminEndpoint()
    {
        await CreateAndAuthenticateAsync(
            "Paulo Professor",
            "paulo.no-admin@prometheus.local",
            "Professor");

        var response = await _client.GetAsync("/api/demo/admin");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Professor_CanAccessProfessorEndpoint()
    {
        await CreateAndAuthenticateAsync(
            "Paulo Professor",
            "paulo.allowed@prometheus.local",
            "Professor");

        var response = await _client.GetAsync("/api/demo/professor");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Professor", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Aluno_CanAccessAlunoEndpoint()
    {
        await CreateAndAuthenticateAsync(
            "Alice Aluno",
            "alice.allowed@prometheus.local",
            "Aluno");

        var response = await _client.GetAsync("/api/demo/aluno");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Aluno", await response.Content.ReadAsStringAsync());
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();

        DeleteIfExists(_factory.DatabasePath);
        DeleteIfExists($"{_factory.DatabasePath}-shm");
        DeleteIfExists($"{_factory.DatabasePath}-wal");
    }

    private async Task AuthenticateAsAdminAsync()
    {
        var response = await LoginAsync("admin@prometheus.local", "Admin@123");
        response.EnsureSuccessStatusCode();
        var login = await ReadAsync<LoginResponse>(response);
        SetBearerToken(login.Token);
    }

    private async Task CreateAndAuthenticateAsync(string name, string email, string role)
    {
        await AuthenticateAsAdminAsync();
        var createResponse = await CreateUserAsync(name, email, "Senha@123", role);
        createResponse.EnsureSuccessStatusCode();

        var loginResponse = await LoginAsync(email, "Senha@123");
        loginResponse.EnsureSuccessStatusCode();
        var login = await ReadAsync<LoginResponse>(loginResponse);
        SetBearerToken(login.Token);
    }

    private Task<HttpResponseMessage> LoginAsync(string email, string password) =>
        _client.PostAsJsonAsync("/api/auth/login", new { email, password });

    private Task<HttpResponseMessage> CreateUserAsync(
        string name,
        string email,
        string password,
        string role) =>
        _client.PostAsJsonAsync("/api/users", new { name, email, password, role });

    private void SetBearerToken(string token)
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    private static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        var value = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        return value ?? throw new InvalidOperationException("A resposta JSON estava vazia.");
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
