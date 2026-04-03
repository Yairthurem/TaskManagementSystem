import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useGetTasksQuery, useDeleteTaskMutation, useGetUsersQuery } from '../store/apiSlice'

export default function TaskList() {
  const navigate = useNavigate()
  const [selectedUserId, setSelectedUserId] = useState('')
  const { data: tasks, isLoading, isError } = useGetTasksQuery()
  const { data: users } = useGetUsersQuery()
  const [deleteTask] = useDeleteTaskMutation()

  if (isLoading) return <div style={{ textAlign: 'center', padding: '3rem', color: 'var(--text-muted)' }}>Loading tasks...</div>
  if (isError) return <div style={{ textAlign: 'center', padding: '3rem' }} className="text-danger">Failed to load tasks. Ensure backend is running.</div>

  const getPriorityBadge = (prio) => {
    switch (prio) {
      case 'High': return <span className="badge high">High</span>;
      case 'Medium': return <span className="badge medium">Medium</span>;
      default: return <span className="badge low">Low</span>;
    }
  }

  // Clone, filter, and sort tasks
  let processedTasks = tasks ? [...tasks] : []
  
  // Optional specific user filter
  if (selectedUserId) {
    processedTasks = processedTasks.filter(task => task.userId === parseInt(selectedUserId))
  }

  // Sort by DueDate descending
  processedTasks.sort((a, b) => {
    if (!a.dueDate) return 1;
    if (!b.dueDate) return -1;
    return new Date(b.dueDate) - new Date(a.dueDate);
  })

  return (
    <div>
      <div className="header">
        <h1>Your Tracking Board</h1>
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
          <label style={{ fontSize: '0.875rem', fontWeight: 500, color: 'var(--text-muted)' }}>Filter by Assignee:</label>
          <select 
            className="form-control" 
            style={{ width: '220px', padding: '0.5rem', appearance: 'menulist' }}
            value={selectedUserId}
            onChange={(e) => setSelectedUserId(e.target.value)}
          >
            <option value="">All Users</option>
            {users?.map(u => (
              <option key={u.id} value={u.id}>{u.firstName} {u.lastName}</option>
            ))}
          </select>
        </div>
      </div>
      
      <div className="card" style={{ padding: '0' }}>
        {processedTasks.length > 0 ? processedTasks.map(task => (
          <div key={task.id} className="task-item">
            <div className="task-content">
              <h3>{task.title}</h3>
              <p>{task.description || "No description provided."}</p>
              <div className="tags-row">
                {task.tags && task.tags.map(tag => (
                   <span key={tag} className="tag-badge">#{tag}</span>
                ))}
              </div>
              <p style={{ marginTop: '0.5rem', fontSize: '0.75rem', color: 'var(--text-muted)', fontWeight: 500 }}>
                Due: {task.dueDate ? new Date(task.dueDate).toLocaleString() : 'No Deadline'}
              </p>
            </div>
            
            <div style={{ display: 'flex', gap: '0.75rem', alignItems: 'center' }}>
              {task.reminderSent && <span className="badge reminded">🔔 Reminded</span>}
              {getPriorityBadge(task.priority)}
              <div style={{ display: 'flex', gap: '0.5rem', marginLeft: '1rem' }}>
                <button 
                  className="btn btn-secondary"
                  onClick={() => navigate('/edit-task/' + task.id)}
                >
                  Edit
                </button>
                <button 
                  className="btn btn-danger"
                  onClick={() => deleteTask(task.id)}
                >
                  Delete
                </button>
              </div>
            </div>
          </div>
        )) : (
          <div style={{ padding: '4rem', textAlign: 'center', color: 'var(--text-muted)' }}>
            No tasks found. Try clearing filters or create a new one!
          </div>
        )}
      </div>
    </div>
  )
}
