interface LoadingScreenProps {
  message?: string
}

export function LoadingScreen({ message = 'Carregando...' }: LoadingScreenProps) {
  return (
    <main className="centered-page" aria-live="polite" aria-busy="true">
      <div className="loading-card">
        <span className="spinner" aria-hidden="true" />
        <p>{message}</p>
      </div>
    </main>
  )
}
