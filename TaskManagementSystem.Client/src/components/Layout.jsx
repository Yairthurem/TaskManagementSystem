import { Link } from 'react-router-dom'

export default function Layout({ children }) {
  return (
    <div>
      <nav style={{ background: 'white', padding: '1rem 0', borderBottom: '1px solid #E5E7EB', marginBottom: '2rem' }}>
        <div className="container" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', margin: '0 auto', maxWidth: '1000px', padding: '0 2rem' }}>
          <Link to="/" style={{ fontSize: '1.25rem', fontWeight: 'bold', color: 'var(--primary-color)', textDecoration: 'none' }}>
            TaskHub Flow
          </Link>
          <div style={{ display: 'flex', gap: '1.5rem', alignItems: 'center' }}>
            <Link to="/" style={{ color: 'var(--text-dark)', textDecoration: 'none', fontWeight: '500' }}>Dashboard</Link>
            <Link to="/new-task" className="btn btn-primary" style={{ textDecoration: 'none' }}>New Task</Link>
          </div>
        </div>
      </nav>
      <main className="container">
        {children}
      </main>
    </div>
  )
}
