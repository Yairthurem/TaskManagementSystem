import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useGetTasksQuery, useDeleteTaskMutation, useGetUsersQuery } from '../store/apiSlice'

export default function TaskList() {
  const navigate = useNavigate()
  const [selectedUserId, setSelectedUserId] = useState('')
  const { data: tasks, isLoading, isError } = useGetTasksQuery(undefined, { 
    pollingInterval: 30000 // Refresh every 30s to catch background updates
  })
  const { data: users } = useGetUsersQuery()
  const [deleteTask] = useDeleteTaskMutation()

  if (isLoading) return <div className="loading-container">Loading tasks...</div>
  if (isError) return <div className="error-container text-danger">Failed to load tasks. Ensure backend is running.</div>

  const getPriorityBadge = (prio) => {
    switch (prio) {
      case 'High': return <span className="badge high">High</span>;
      case 'Medium': return <span className="badge medium">Medium</span>;
      default: return <span className="badge low">Low</span>;
    }
  }

  // Priority weights for sorting
  const priorityMap = { 'High': 2, 'Medium': 1, 'Low': 0 }

  // Clone, filter, and sort tasks
  let processedTasks = tasks ? [...tasks] : []
  
  // Optional specific user filter
  if (selectedUserId) {
    processedTasks = processedTasks.filter(task => task.userId === parseInt(selectedUserId))
  }

  // Sort by Priority DESC, then DueDate DESC
  processedTasks.sort((a, b) => {
    const pA = priorityMap[a.priority] ?? 0
    const pB = priorityMap[b.priority] ?? 0
    if (pB !== pA) return pB - pA // High priority first
    
    // Sort by DueDate ascending (nearest first) if priorities match
    // Tasks with no deadline are pushed to the very bottom
    const dateA = a.dueDate ? new Date(a.dueDate).getTime() : Number.MAX_SAFE_INTEGER;
    const dateB = b.dueDate ? new Date(b.dueDate).getTime() : Number.MAX_SAFE_INTEGER;
    return dateA - dateB;
  })

  return (
    <div>
      <div className="header">
        <h1>Your Tracking Board</h1>
        <div className="d-flex-gap">
          <label className="filter-label">Filter by Assignee:</label>
          <select 
            className="filter-select" 
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
      
      <div className="card p-0">
        <div className="task-scroll-container">
          {processedTasks.length > 0 ? processedTasks.map(task => (
            <div key={task.id} className="task-item">
              <div className="task-content">
                <h3>{task.title}</h3>
                <p className="task-description">{task.description || "No description provided."}</p>
                {!selectedUserId && (
                  <p className="task-assignee">
                    Assigned to: <span>
                      {(() => {
                        const user = users?.find(u => u.id === task.userId);
                        return user ? `${user.firstName} ${user.lastName}` : 'System';
                      })()}
                    </span>
                  </p>
                )}
                <div className="tags-row">
                  {task.tags && task.tags.map(tag => (
                    <span key={tag} className="tag-badge">#{tag}</span>
                  ))}
                </div>
                <div className="task-footer">
                  <p className="task-due-date">
                    Due: {task.dueDate ? new Date(task.dueDate).toLocaleString() : 'No Deadline'}
                  </p>
                  {task.reminderSent && <span className="badge reminded">🔔 REMINDED</span>}
                </div>
              </div>
              
              <div className="d-flex-gap">
                {getPriorityBadge(task.priority)}
                <div className="d-flex-gap-sm">
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
            <div className="loading-container p-4">
              No tasks found. Try clearing filters or create a new one!
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
