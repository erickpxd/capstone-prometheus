import { useEffect, useState } from 'react'
import { api } from '../api/client'
import { getApiErrorMessage } from '../api/errors'
import { useAuth } from '../auth/useAuth'
import { AccessChecks } from '../components/AccessChecks'
import { AuthenticatedLayout } from '../layouts/AuthenticatedLayout'
import type { DemoResponse, UserRole } from '../types/auth'

interface RoleDashboardProps {
  role: Extract<UserRole, 'Professor' | 'Aluno'>
  endpoint: '/api/demo/professor' | '/api/demo/aluno'
}

export function RoleDashboard({ role, endpoint }: RoleDashboardProps) {
  const { user } = useAuth()
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    let isActive = true

    api
      .get<DemoResponse>(endpoint)
      .then(({ data }) => {
        if (isActive) {
          setMessage(data.message)
        }
      })
      .catch((requestError) => {
        if (isActive) {
          setError(getApiErrorMessage(requestError, `Não foi possível validar o acesso de ${role}.`))
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
  }, [endpoint, role])

  return (
    <AuthenticatedLayout>
      <header className="page-heading">
        <div>
          <p className="eyebrow">Área do {role}</p>
          <h1>Bem-vindo, {user?.name}.</h1>
          <p>Você está autenticado como {role}.</p>
        </div>
      </header>

      <section className="role-dashboard">
        <article className="card access-card access-card--large">
          <div className="card__heading">
            <div>
              <p className="eyebrow">Autorização</p>
              <h2>Acesso protegido</h2>
            </div>
            <span className={`role-badge role-${role.toLowerCase()}`}>{role}</span>
          </div>
          <AccessChecks role={role} authorized={Boolean(message)} />
          {isLoading && <p className="muted" aria-live="polite">Validando com a API...</p>}
          {message && <p className="api-message" role="status">{message}</p>}
          {error && <div className="alert alert--error" role="alert">{error}</div>}
        </article>

        <aside className="card info-card">
          <p className="eyebrow">Escopo demonstrativo</p>
          <h2>Sessão ativa</h2>
          <p>
            Esta área confirma a autenticação por JWT e a autorização por perfil.
            Outras funções acadêmicas não fazem parte desta implementação referencial.
          </p>
        </aside>
      </section>
    </AuthenticatedLayout>
  )
}
