import { Route, Routes } from 'react-router-dom'
import { AdminDashboard } from './pages/AdminDashboard'
import { AlunoDashboard } from './pages/AlunoDashboard'
import { LoginPage } from './pages/LoginPage'
import { ProfessorDashboard } from './pages/ProfessorDashboard'
import { UnauthorizedPage } from './pages/UnauthorizedPage'
import { ProtectedRoute } from './routes/ProtectedRoute'
import { RoleHomeRedirect } from './routes/RoleHomeRedirect'

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<RoleHomeRedirect />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/unauthorized" element={<UnauthorizedPage />} />

      <Route element={<ProtectedRoute allowedRoles={['Admin']} />}>
        <Route path="/admin" element={<AdminDashboard />} />
      </Route>

      <Route element={<ProtectedRoute allowedRoles={['Professor']} />}>
        <Route path="/professor" element={<ProfessorDashboard />} />
      </Route>

      <Route element={<ProtectedRoute allowedRoles={['Aluno']} />}>
        <Route path="/aluno" element={<AlunoDashboard />} />
      </Route>

      <Route path="*" element={<RoleHomeRedirect />} />
    </Routes>
  )
}
