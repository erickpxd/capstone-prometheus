import { useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/useAuth'
import { Brand } from '../components/Brand'
import { getRolePath } from '../routes/rolePath'

export function UnauthorizedPage() {
  const { role } = useAuth()
  const navigate = useNavigate()

  function goBack() {
    navigate(role ? getRolePath(role) : '/login', { replace: true })
  }

  return (
    <main className="centered-page">
      <section className="message-card" aria-labelledby="unauthorized-title">
        <Brand compact />
        <span className="message-card__icon" aria-hidden="true">!</span>
        <h1 id="unauthorized-title">Acesso não autorizado</h1>
        <p>Seu perfil não possui permissão para acessar esta área.</p>
        <button className="button button--primary" type="button" onClick={goBack}>
          Voltar
        </button>
      </section>
    </main>
  )
}
