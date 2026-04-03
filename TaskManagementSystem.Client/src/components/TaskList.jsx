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
        <div className="task-scroll-container">
          {processedTasks.length > 0 ? processedTasks.map(task => (
            <div key={task.id} className="task-item">
              <div className="task-content">
                <h3>{task.title}</h3>
                <p style={{ fontSize: '0.8rem' }}>{task.description || "No description provided."}</p>
                <div className="tags-row">
                  {task.tags && task.tags.map(tag => (
                    <span key={tag} className="tag-badge">#{tag}</span>
                  ))}
                </div>
                <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', marginTop: '0.4rem' }}>
                  <p style={{ fontSize: '0.7rem', color: 'var(--text-muted)', fontWeight: 500 }}>
                    Due: {task.dueDate ? new Date(task.dueDate).toLocaleString() : 'No Deadline'}
                  </p>
                  {task.reminderSent && <span className="badge reminded">🔔 REMINDED</span>}
                </div>
              </div>
              
              <div style={{ display: 'flex', gap: '0.6rem', alignItems: 'center' }}>
                {getPriorityBadge(task.priority)}
                <div style={{ display: 'flex', gap: '0.35rem', marginLeft: '0.5rem' }}>
                  <button 
                    className="icon-btn"
                    title="Edit Task"
                    onClick={() => navigate('/edit-task/' + task.id)}
                  >
                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z"></path></svg>
                  </button>
                  <button 
                    className="icon-btn delete"
                    title="Delete Task"
                    onClick={() => window.confirm("Are you sure you want to delete this task?") && deleteTask(task.id)}
                  >
                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-4v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
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
    </div>
  )
}
