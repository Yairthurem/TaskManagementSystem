import { useEffect, useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { useNavigate, useParams } from 'react-router-dom'
import { useCreateTaskMutation, useUpdateTaskMutation, useGetTasksQuery, useGetUsersQuery, useGetTagsQuery } from '../store/apiSlice'

const taskSchema = z.object({
  title: z.string().min(1, "Title is required").max(100),
  description: z.string().max(500).optional(),
  dueDate: z.string().optional().or(z.literal('')).refine((val) => {
    if (!val) return true;
    const date = new Date(val);
    const now = new Date();
    // Allow if same minute or future (buffer of 60s for slow typing)
    return date.getTime() >= now.getTime() - 60000;
  }, {
    message: "Due date cannot be in the past"
  }),
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
          const offset = date.getTimezoneOffset() * 60000;
          localDateString = new Date(date - offset).toISOString().slice(0, 16)
        }

        reset({
          title: taskToEdit.title,
          description: taskToEdit.description || '',
          dueDate: localDateString,
          userId: taskToEdit.userId ? taskToEdit.userId.toString() : '',
          priority: taskToEdit.priority,
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
  if (!isDataLoaded) return <div className="loading-container">Preparing form...</div>

  const isSaving = isCreating || isUpdating

  // Generate local date string for 'min' attribute so past days are greyed out
  const getMinDate = () => {
    if (isEditMode) return undefined;
    const now = new Date();
    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0');
    const day = String(now.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}T00:00`;
  };

  const minDate = getMinDate();

  return (
    <div className="card form-card">
      <h2 className="form-title">
        {isEditMode ? 'Editing Task' : 'Create New Task'}
      </h2>
      
      <form onSubmit={handleSubmit(onSubmit)}>
        <div className="form-group">
          <label htmlFor="title" className="form-label">Task Title</label>
          <input id="title" className="form-control" {...register('title')} placeholder="e.g., Run security audits" />
          {errors.title && <span className="text-danger">{errors.title.message}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="description" className="form-label">Description (Optional)</label>
          <textarea id="description" className="form-control" {...register('description')} rows="3" placeholder="Additional details..." />
          {errors.description && <span className="text-danger">{errors.description.message}</span>}
        </div>

        <div className="form-row">
            <div>
              <label htmlFor="dueDate" className="form-label">Due Date (Optional)</label>
              <input 
                id="dueDate"
                type="datetime-local" 
                className="form-control" 
                {...register('dueDate')} 
                min={minDate}
              />
              {errors.dueDate && <span className="text-danger">{errors.dueDate.message}</span>}
            </div>

            <div>
              <label htmlFor="priority" className="form-label">Priority</label>
              <select id="priority" className="form-control" {...register('priority')}>
                <option value="Low">Low</option>
                <option value="Medium">Medium</option>
                <option value="High">High</option>
              </select>
            </div>
        </div>

        <div className="form-group">
          <label htmlFor="userId" className="form-label">Assign To</label>
          <select id="userId" className="form-control" {...register('userId')}>
            <option value="">Select a user...</option>
            {users && users.map(u => (
              <option key={u.id} value={u.id}>{u.firstName} {u.lastName}</option>
            ))}
          </select>
          {errors.userId && <span className="text-danger">{errors.userId.message}</span>}
          {loadingUsers && <span className="helper-text">Loading users from DB...</span>}
        </div>
        
        <div className="form-group">
          <label htmlFor="tagIds" className="form-label">Tags</label>
          <select id="tagIds" className="form-control tag-select-field" multiple {...register('tagIds')}>
            {tags && tags.map(t => (
              <option key={t.id} value={t.id}>{t.name}</option>
            ))}
          </select>
          <p className="helper-text">Hold Ctrl/Cmd to select multiple tags.</p>
        </div>

        <button type="submit" className="btn btn-primary w-full" disabled={isSaving}>
          {isSaving ? 'Synchronizing...' : (isEditMode ? 'Save Changes' : 'Create Task')}
        </button>
      </form>
    </div>
  )
}
