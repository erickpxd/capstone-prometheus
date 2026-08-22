interface BrandProps {
  compact?: boolean
}

export function Brand({ compact = false }: BrandProps) {
  return (
    <div className={compact ? 'brand brand--compact' : 'brand'}>
      <span className="brand__mark" aria-hidden="true">
        P
      </span>
      <span>
        <strong>PROMETHEUS</strong>
        <small>Academic Platform</small>
      </span>
    </div>
  )
}
