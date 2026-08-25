import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { api } from '../api/client'
import { getApiErrorMessage } from '../api/errors'
import { useAuth } from '../auth/useAuth'
import { AuthenticatedLayout } from '../layouts/AuthenticatedLayout'
import type {
  CreateUserRequest,
  CreatableRole,
  User,
} from '../types/auth'

interface UserFormState {
  name: string
  email: string
  password: string
}

const emptyForm: UserFormState = {
  name: '',
  email: '',
  password: '',
}

export function AdminDashboard() {
  const { user } = useAuth()
  const [users, setUsers] = useState<User[]>([])
  const [selectedRole, setSelectedRole] = useState<CreatableRole>('Aluno')
  const [form, setForm] = useState<UserFormState>(emptyForm)
  const [loadError, setLoadError] = useState('')
  const [formError, setFormError] = useState('')
  const [success, setSuccess] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    let isActive = true

    api
      .get<User[]>('/api/users')
      .then(({ data }) => {
        if (isActive) {
          setUsers(data)
        }
      })
      .catch((requestError) => {
        if (isActive) {
          setLoadError(getApiErrorMessage(requestError, 'Não foi possível carregar a área administrativa.'))
        }
      })
      .finally(() => {
        if (isActive) {
          setIsLoading(false)
        }
      })

    return () => {
      isActive = false
    }
  }, [])

  function selectRole(role: CreatableRole) {
    setSelectedRole(role)
    setFormError('')
    setSuccess('')
  }

  function updateField(field: keyof UserFormState, value: string) {
    setForm((current) => ({ ...current, [field]: value }))
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setFormError('')
    setSuccess('')
    setIsSubmitting(true)

    const request: CreateUserRequest = {
      ...form,
      role: selectedRole,
    }

    try {
      const { data: createdUser } = await api.post<User>('/api/users', request)
      setUsers((currentUsers) => [...currentUsers, createdUser])
      setSuccess(`${request.role} cadastrado com sucesso.`)
      setForm(emptyForm)
    } catch (requestError) {
      setFormError(getApiErrorMessage(requestError, 'Não foi possível cadastrar o usuário.'))
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <AuthenticatedLayout>
      <header className="page-heading">
        <div>
          <p className="eyebrow">Administração</p>
          <h1>Bem-vindo, {user?.name}.</h1>
          <p>Gerencie as contas de Professores e Alunos.</p>
        </div>
      </header>

      <section className="dashboard-grid">
        <article className="card create-user-card">
          <div className="card__heading">
            <div>
              <p className="eyebrow">Novo usuário</p>
              <h2>Cadastrar {selectedRole}</h2>
            </div>
            <span className={`role-badge role-${selectedRole.toLowerCase()}`}>{selectedRole}</span>
          </div>

          <div className="segmented-control" aria-label="Perfil do novo usuário">
            <button
              type="button"
              className={selectedRole === 'Aluno' ? 'is-selected' : ''}
              aria-pressed={selectedRole === 'Aluno'}
              disabled={isSubmitting}
              onClick={() => selectRole('Aluno')}
            >
              Cadastrar Aluno
            </button>
            <button
              type="button"
              className={selectedRole === 'Professor' ? 'is-selected' : ''}
              aria-pressed={selectedRole === 'Professor'}
              disabled={isSubmitting}
              onClick={() => selectRole('Professor')}
            >
              Cadastrar Professor
            </button>
          </div>

          <form className="form-grid" onSubmit={handleSubmit}>
            <label htmlFor="new-user-name">Nome</label>
            <input
              id="new-user-name"
              type="text"
              autoComplete="name"
              value={form.name}
              onChange={(event) => updateField('name', event.target.value)}
              required
            />

            <label htmlFor="new-user-email">E-mail</label>
            <input
              id="new-user-email"
              type="email"
              autoComplete="email"
              value={form.email}
              onChange={(event) => updateField('email', event.target.value)}
              required
            />

            <label htmlFor="new-user-password">Senha</label>
            <input
              id="new-user-password"
              type="password"
              autoComplete="new-password"
              minLength={8}
              value={form.password}
              onChange={(event) => updateField('password', event.target.value)}
              required
            />
            <small className="field-hint">Mínimo de 8 caracteres.</small>

            {formError && <div className="alert alert--error form-grid__wide" role="alert">{formError}</div>}
            {success && <div className="alert alert--success form-grid__wide" role="status">{success}</div>}

            <button className="button button--primary form-grid__wide" type="submit" disabled={isSubmitting}>
              {isSubmitting ? 'Cadastrando...' : `Cadastrar ${selectedRole}`}
            </button>
          </form>
        </article>
      </section>

      <section className="card users-card" aria-labelledby="users-title">
        <div className="card__heading">
          <div>
            <p className="eyebrow">Diretório</p>
            <h2 id="users-title">Usuários cadastrados</h2>
          </div>
          <span className="count-badge">{users.length}</span>
        </div>

        {isLoading && <p className="muted" aria-live="polite">Carregando usuários...</p>}
        {loadError && <div className="alert alert--error" role="alert">{loadError}</div>}

        <div className="table-wrapper">
          <table>
            <thead>
              <tr>
                <th>Nome</th>
                <th>E-mail</th>
                <th>Perfil</th>
                <th>Criado em</th>
              </tr>
            </thead>
            <tbody>
              {users.map((listedUser) => (
                <tr key={listedUser.id}>
                  <td>{listedUser.name}</td>
                  <td>{listedUser.email}</td>
                  <td>
                    <span className={`role-badge role-${listedUser.role.toLowerCase()}`}>
                      {listedUser.role}
                    </span>
                  </td>
                  <td>{formatDate(listedUser.createdAt)}</td>
                </tr>
              ))}
            </tbody>
          </table>
          {!isLoading && users.length === 0 && !loadError && (
            <p className="empty-state">Nenhum usuário encontrado.</p>
          )}
        </div>
      </section>
    </AuthenticatedLayout>
  )
}

function formatDate(value: string): string {
  const date = new Date(value)

  if (Number.isNaN(date.getTime())) {
    return '—'
  }

  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'short',
    timeStyle: 'short',
  }).format(date)
}
