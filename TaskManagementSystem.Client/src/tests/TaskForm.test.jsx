import { render, screen, fireEvent } from '@testing-library/react'
import { describe, it, expect, vi } from 'vitest'
import { MemoryRouter } from 'react-router-dom'
import TaskForm from '../components/TaskForm'
import * as apiSlice from '../store/apiSlice'

// Mock the API hooks
vi.mock('../store/apiSlice', async () => {
  const actual = await vi.importActual('../store/apiSlice')
  return {
    ...actual,
    useGetUsersQuery: vi.fn(),
    useGetTagsQuery: vi.fn(),
    useGetTasksQuery: vi.fn(),
    useCreateTaskMutation: vi.fn(),
    useUpdateTaskMutation: vi.fn(),
  }
})

describe('TaskForm Component', () => {
  it('renders correctly in creation mode', async () => {
    // Mock successful data loading
    apiSlice.useGetUsersQuery.mockReturnValue({ data: [{ id: 1, firstName: 'John', lastName: 'Doe' }], isLoading: false })
    apiSlice.useGetTagsQuery.mockReturnValue({ data: [{ id: 1, name: 'Work' }], isLoading: false })
    apiSlice.useGetTasksQuery.mockReturnValue({ data: [], isLoading: false })
    apiSlice.useCreateTaskMutation.mockReturnValue([vi.fn(), { isLoading: false }])
    apiSlice.useUpdateTaskMutation.mockReturnValue([vi.fn(), { isLoading: false }])
    
    render(
      <MemoryRouter>
        <TaskForm />
      </MemoryRouter>
    )

    expect(screen.getByText('Create New Task')).toBeInTheDocument()
    expect(screen.getByPlaceholderText(/Run security audits/i)).toBeInTheDocument()
  })

  it('shows validation errors for empty fields', async () => {
    apiSlice.useGetUsersQuery.mockReturnValue({ data: [], isLoading: false })
    apiSlice.useGetTagsQuery.mockReturnValue({ data: [], isLoading: false })
    apiSlice.useGetTasksQuery.mockReturnValue({ data: [], isLoading: false })
    apiSlice.useCreateTaskMutation.mockReturnValue([vi.fn(), { isLoading: false }])
    apiSlice.useUpdateTaskMutation.mockReturnValue([vi.fn(), { isLoading: false }])

    render(
      <MemoryRouter>
        <TaskForm />
      </MemoryRouter>
    )

    const submitBtn = screen.getByText('Create Task')
    fireEvent.click(submitBtn)

    expect(await screen.findByText(/Title is required/i)).toBeInTheDocument()
    expect(await screen.findByText(/Please assign a user/i)).toBeInTheDocument()
  })
})
