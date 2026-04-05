import { screen, fireEvent, waitFor } from '@testing-library/react'
import { describe, it, expect, vi } from 'vitest'
import TaskList from '../components/TaskList'
import { renderWithProviders } from './test-utils'
import { http, HttpResponse } from 'msw'
import { server } from './msw/server'

const API_BASE_URL = 'http://localhost:5050/api/'

describe('TaskList Component with MSW', () => {
  const mockTasks = [
    { id: 1, title: 'Task 1', priority: 'High', status: 'ToDo', userId: 1, user: { firstName: 'John' }, tags: ['Work'] },
    { id: 2, title: 'Task 2', priority: 'Low', status: 'Done', userId: 1, user: { firstName: 'John' }, tags: [] }
  ]

  it('renders loading state', async () => {
    server.use(
      http.get('*/tasks', () => {
        return new Promise(() => {}) // Never resolves
      })
    )
    renderWithProviders(<TaskList />)
    expect(screen.getByText(/Loading tasks.../i)).toBeInTheDocument()
  })

  it('renders error state', async () => {
    server.use(
      http.get('*/tasks', () => {
        return new HttpResponse(null, { status: 500 })
      })
    )
    renderWithProviders(<TaskList />)
    expect(await screen.findByText(/Failed to load tasks/i)).toBeInTheDocument()
  })

  it('renders tasks correctly', async () => {
    // Uses default handler in msw/handlers.js (+ our override here for specificity)
    server.use(
      http.get('*/tasks', () => {
        return HttpResponse.json(mockTasks)
      })
    )
    
    renderWithProviders(<TaskList />)

    expect(await screen.findByText('Task 1')).toBeInTheDocument()
    expect(screen.getByText('Task 2')).toBeInTheDocument()
    expect(screen.getByText('#Work')).toBeInTheDocument()
  })

  it('shows empty state when no tasks available', async () => {
    server.use(
      http.get('*/tasks', () => {
        return HttpResponse.json([])
      })
    )
    renderWithProviders(<TaskList />)
    expect(await screen.findByText(/No tasks found/i)).toBeInTheDocument()
  })

  it('calls delete mutation when delete button is clicked', async () => {
    server.use(
      http.get('*/tasks', () => {
        return HttpResponse.json(mockTasks)
      })
    )
    
    let deleteCalled = false
    server.use(
      http.delete('*/tasks/:id', ({ params }) => {
        if (params.id === '1') deleteCalled = true
        return new HttpResponse(null, { status: 204 })
      })
    )

    // Mock window.confirm to return true
    const confirmSpy = vi.spyOn(window, 'confirm').mockImplementation(() => true)
    
    renderWithProviders(<TaskList />)

    const deleteButtons = await screen.findAllByTitle(/Delete Task/i)
    fireEvent.click(deleteButtons[0])

    await waitFor(() => {
      expect(deleteCalled).toBe(true)
    })
    confirmSpy.mockRestore()
  })
})
