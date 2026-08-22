import type { ReactNode } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/useAuth'
import { Brand } from '../components/Brand'

interface AuthenticatedLayoutProps {
  children: ReactNode
}

export function AuthenticatedLayout({ children }: AuthenticatedLayoutProps) {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  function handleLogout() {
    logout()
    navigate('/login', { replace: true })
  }

  return (
    <div className="app-shell">
      <header className="app-header">
        <Brand compact />
        <div className="app-header__user">
          <div>
            <span className="user-name">{user?.name}</span>
            {user && <span className={`role-badge role-${user.role.toLowerCase()}`}>{user.role}</span>}
          </div>
          <button className="button button--secondary" type="button" onClick={handleLogout}>
            Sair
          </button>
        </div>
      </header>
      <main className="content-container">{children}</main>
    </div>
  )
}
