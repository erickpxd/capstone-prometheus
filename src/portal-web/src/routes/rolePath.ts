import type { UserRole } from '../types/auth'

const rolePaths: Record<UserRole, string> = {
  Admin: '/admin',
  Professor: '/professor',
  Aluno: '/aluno',
}

export function getRolePath(role: UserRole): string {
  return rolePaths[role]
}
