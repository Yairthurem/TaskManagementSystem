import { Link } from 'react-router-dom'

export default function Layout({ children }) {
  return (
    <div>
      <nav className="navbar">
        <div className="container nav-container">
          <Link to="/" className="nav-logo">
            TaskHub Flow
          </Link>
          <div className="nav-links">
            <Link to="/" className="nav-link">Dashboard</Link>
            <Link to="/new-task" className="btn btn-primary">New Task</Link>
          </div>
        </div>
      </nav>
      <main className="container">
        {children}
      </main>
    </div>
  )
}
