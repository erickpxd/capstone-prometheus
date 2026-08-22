import type { UserRole } from '../types/auth'

interface AccessChecksProps {
  role: UserRole
  authorized: boolean
}

export function AccessChecks({ role, authorized }: AccessChecksProps) {
  const checks = [
    'JWT válido',
    `Perfil ${role}`,
    role === 'Admin'
      ? 'Acesso administrativo autorizado'
      : 'Acesso autorizado',
  ]

  return (
    <ul className="check-list" aria-label="Validação de acesso">
      {checks.map((check) => (
        <li key={check} className={authorized ? 'is-valid' : ''}>
          <span aria-hidden="true">{authorized ? '✓' : '○'}</span>
          {check}
        </li>
      ))}
    </ul>
  )
}
