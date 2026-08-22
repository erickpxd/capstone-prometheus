import { Navigate } from 'react-router-dom'
import { useAuth } from '../auth/useAuth'
import { LoadingScreen } from '../components/LoadingScreen'
import { getRolePath } from './rolePath'

export function RoleHomeRedirect() {
  const { isAuthenticated, isLoading, role } = useAuth()

  if (isLoading) {
    return <LoadingScreen message="Restaurando sessão..." />
  }

  if (!isAuthenticated || !role) {
    return <Navigate to="/login" replace />
  }

  return <Navigate to={getRolePath(role)} replace />
}
