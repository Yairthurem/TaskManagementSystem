import { render, screen } from '@testing-library/react'
import { describe, it, expect, vi } from 'vitest'
import { Provider } from 'react-redux'
import { store } from './store/store'
import App from './App'
import * as apiSlice from './store/apiSlice'

// Mock the API hooks to avoid loading states that break the test
vi.mock('./store/apiSlice', async () => {
  const actual = await vi.importActual('./store/apiSlice')
  return {
    ...actual,
    useGetUsersQuery: vi.fn().mockReturnValue({ data: [], isLoading: false }),
    useGetTagsQuery: vi.fn().mockReturnValue({ data: [], isLoading: false }),
    useGetTasksQuery: vi.fn().mockReturnValue({ data: [], isLoading: false }),
    useCreateTaskMutation: vi.fn().mockReturnValue([vi.fn(), { isLoading: false }]),
    useUpdateTaskMutation: vi.fn().mockReturnValue([vi.fn(), { isLoading: false }]),
    useDeleteTaskMutation: vi.fn().mockReturnValue([vi.fn(), { isLoading: false }]),
  }
})

describe('App React Ecosystem', () => {
  it('renders the initial layout wrapper beautifully', () => {
    render(
      <Provider store={store}>
        <App />
      </Provider>
    )
    
    // The Layout Component natively renders this semantic title
    expect(screen.getByText(/TaskHub Flow/i)).toBeInTheDocument()

    // Proving react-router-dom physically defaults to TaskList view
    expect(screen.getByText(/Your Tracking Board/i)).toBeInTheDocument()
  })
})
