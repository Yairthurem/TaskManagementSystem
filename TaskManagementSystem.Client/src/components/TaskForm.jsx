import { useEffect, useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { useNavigate, useParams } from 'react-router-dom'
import { useCreateTaskMutation, useUpdateTaskMutation, useGetTasksQuery, useGetUsersQuery, useGetTagsQuery } from '../store/apiSlice'

const taskSchema = z.object({
  title: z.string().min(1, "Title is required").max(100),
  description: z.string().max(500).optional(),
  dueDate: z.string().optional().or(z.literal('')),
  priority: z.enum(['Low', 'Medium', 'High']),
  userId: z.string().min(1, "Please assign a user"),
  tagIds: z.array(z.string()).optional()
})

export default function TaskForm() {
  const { id } = useParams()
  const isEditMode = !!id
  const navigate = useNavigate()

  const { data: users, isLoading: loadingUsers } = useGetUsersQuery()
  const { data: tags, isLoading: loadingTags } = useGetTagsQuery()
  const { data: allTasks } = useGetTasksQuery(undefined, { skip: !isEditMode })
  
  const [createTask, { isLoading: isCreating }] = useCreateTaskMutation()
  const [updateTask, { isLoading: isUpdating }] = useUpdateTaskMutation()
  const [isDataLoaded, setIsDataLoaded] = useState(false)

  const { register, handleSubmit, reset, formState: { errors } } = useForm({
    resolver: zodResolver(taskSchema),
    defaultValues: { priority: 'Low', tagIds: [] }
  })

  useEffect(() => {
    if (isEditMode && allTasks && tags) {
      const taskToEdit = allTasks.find(t => t.id === parseInt(id))
      if (taskToEdit) {
        // Convert UTC string to Date object and then to local YYYY-MM-DDTHH:mm
        let localDateString = ''
        if (taskToEdit.dueDate) {
          const date = new Date(taskToEdit.dueDate)
          const localTime = new Date(date.getTime() - date.getTimezoneOffset() * 60000)
          localDateString = localTime.toISOString().slice(0, 16)
        }

        reset({
          title: taskToEdit.title,
          description: taskToEdit.description || '',
          dueDate: localDateString,
          priority: taskToEdit.priority,
          userId: taskToEdit.userId.toString(),
          tagIds: taskToEdit.tags ? tags?.filter(t => taskToEdit.tags.includes(t.name)).map(t => t.id.toString()) : []
        })
        setIsDataLoaded(true)
      }
    } else if (!isEditMode) {
        // Reset to initial defaults when navigating to 'New Task' while already mounted
        reset({ title: '', description: '', dueDate: '', priority: 'Low', userId: '', tagIds: [] })
        setIsDataLoaded(true)
    }
  }, [isEditMode, allTasks, tags, id, reset])

  const onSubmit = async (data) => {
    const payload = {
      ...data,
      userId: parseInt(data.userId),
      tagIds: data.tagIds ? data.tagIds.map(tagId => parseInt(tagId)) : [],
      dueDate: data.dueDate ? new Date(data.dueDate).toISOString() : null
    }

    try {
      if (isEditMode) {
        await updateTask({ id: parseInt(id), ...payload }).unwrap()
      } else {
        await createTask(payload).unwrap()
      }
      navigate('/')
    } catch (err) {
      // Error handled by global rtkQueryErrorLogger middleware
    }
  }

  // Prevent rendering the form before we reset its default values during edit mode
  if (!isDataLoaded) return <div style={{ textAlign: 'center', padding: '3rem' }}>Preparing form...</div>

  const isSaving = isCreating || isUpdating

  return (
    <div className="card" style={{ maxWidth: '600px', margin: '0 auto' }}>
      <h2 style={{ marginBottom: '1.5rem', fontWeight: 600 }}>
        {isEditMode ? 'Editing Task' : 'Create New Task'}
      </h2>
      
      <form onSubmit={handleSubmit(onSubmit)}>
        <div className="form-group">
          <label className="form-label">Task Title</label>
          <input className="form-control" {...register('title')} placeholder="e.g., Run security audits" />
          {errors.title && <span className="text-danger">{errors.title.message}</span>}
        </div>

        <div className="form-group">
          <label className="form-label">Description (Optional)</label>
          <textarea className="form-control" {...register('description')} rows="3" placeholder="Additional details..." />
          {errors.description && <span className="text-danger">{errors.description.message}</span>}
        </div>

        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem', marginBottom: '1.5rem' }}>
            <div>
              <label className="form-label">Due Date (Optional)</label>
              <input type="datetime-local" className="form-control" {...register('dueDate')} />
              {errors.dueDate && <span className="text-danger">{errors.dueDate.message}</span>}
            </div>

            <div>
              <label className="form-label">Priority</label>
              <select className="form-control" {...register('priority')}>
                <option value="Low">Low</option>
                <option value="Medium">Medium</option>
                <option value="High">High</option>
              </select>
            </div>
        </div>

        <div className="form-group">
          <label className="form-label">Assign To</label>
          <select className="form-control" {...register('userId')}>
            <option value="">Select a user...</option>
            {users && users.map(u => (
              <option key={u.id} value={u.id}>{u.firstName} {u.lastName}</option>
            ))}
          </select>
          {errors.userId && <span className="text-danger">{errors.userId.message}</span>}
          {loadingUsers && <span className="text-muted" style={{ fontSize: '0.8rem' }}>Loading users from DB...</span>}
        </div>
        
        <div className="form-group">
          <label className="form-label">Tags</label>
          <select className="form-control" multiple {...register('tagIds')} style={{ height: '90px', fontSize: '0.8rem' }}>
            {tags && tags.map(t => (
              <option key={t.id} value={t.id}>{t.name}</option>
            ))}
          </select>
          <p style={{ fontSize: '0.75rem', color: 'var(--text-muted)', marginTop: '0.35rem' }}>Hold Ctrl/Cmd to select multiple tags.</p>
        </div>

        <button type="submit" className="btn btn-primary" style={{ width: '100%' }} disabled={isSaving}>
          {isSaving ? 'Synchronizing...' : (isEditMode ? 'Save Changes' : 'Create Task')}
        </button>
      </form>
    </div>
  )
}
