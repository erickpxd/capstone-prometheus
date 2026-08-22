import { useState } from 'react'
import type { FormEvent } from 'react'
import { Navigate, useNavigate } from 'react-router-dom'
import { getApiErrorMessage } from '../api/errors'
import { useAuth } from '../auth/useAuth'
import { Brand } from '../components/Brand'
import { LoadingScreen } from '../components/LoadingScreen'
import { getRolePath } from '../routes/rolePath'

export function LoginPage() {
  const { isAuthenticated, isLoading, login, role } = useAuth()
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  if (isLoading) {
    return <LoadingScreen message="Restaurando sessão..." />
  }

  if (isAuthenticated && role) {
    return <Navigate to={getRolePath(role)} replace />
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError('')
    setIsSubmitting(true)

    try {
      const authenticatedUser = await login({ email, password })
      navigate(getRolePath(authenticatedUser.role), { replace: true })
    } catch (requestError) {
      setError(getApiErrorMessage(requestError, 'Não foi possível entrar.'))
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <main className="login-page">
      <section className="login-panel" aria-labelledby="login-title">
        <Brand />
        <div className="login-panel__intro">
          <p className="eyebrow">Acesso institucional</p>
          <h1 id="login-title">Entre na sua conta</h1>
          <p>Use as credenciais fornecidas pela administração acadêmica.</p>
        </div>

        <form className="form-stack" onSubmit={handleSubmit}>
          <label htmlFor="email">E-mail</label>
          <input
            id="email"
            name="email"
            type="email"
            autoComplete="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            required
            autoFocus
          />

          <label htmlFor="password">Senha</label>
          <input
            id="password"
            name="password"
            type="password"
            autoComplete="current-password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            required
          />

          {error && (
            <div className="alert alert--error" role="alert">
              {error}
            </div>
          )}

          <button className="button button--primary button--full" type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Entrando...' : 'Entrar'}
          </button>
        </form>

        <p className="login-panel__note">Não existe cadastro público. Contas são criadas pelo Admin.</p>
      </section>
      <aside className="login-visual" aria-hidden="true">
        <div>
          <span className="login-visual__number">V2</span>
          <p>Identidade, autenticação e autorização demonstradas de ponta a ponta.</p>
        </div>
      </aside>
    </main>
  )
}
