import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import Layout from './components/Layout'
import TaskList from './components/TaskList'
import TaskForm from './components/TaskForm'

function App() {
  return (
    <Router>
      <Layout>
        <Routes>
          <Route path="/" element={<TaskList />} />
          <Route path="/new-task" element={<TaskForm />} />
          <Route path="/edit-task/:id" element={<TaskForm />} />
        </Routes>
      </Layout>
    </Router>
  )
}

export default App
