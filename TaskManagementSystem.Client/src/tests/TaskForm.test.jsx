import { screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect } from 'vitest'
import TaskForm from '../components/TaskForm'
import { renderWithProviders } from './test-utils'
import { http, HttpResponse } from 'msw'
import { server } from './msw/server'

describe('TaskForm Component with MSW', () => {
  it('renders all form fields correctly', async () => {
    renderWithProviders(<TaskForm />)
    
    expect(await screen.findByRole('heading', { name: /Create New Task/i })).toBeInTheDocument()
    expect(screen.getByLabelText(/Task Title/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/Assign To/i)).toBeInTheDocument()
    
    const userOption = await screen.findByText('Bob Smith')
    expect(userOption).toBeInTheDocument()
  })

  it('shows validation errors for required fields on empty submit', async () => {
    const user = userEvent.setup()
    renderWithProviders(<TaskForm />)

    await screen.findByRole('heading', { name: /Create New Task/i })
    
    const submitBtn = screen.getByRole('button', { name: /Create Task/i })
    await user.click(submitBtn)

    expect(await screen.findByText(/Title is required/i)).toBeInTheDocument()
    expect(await screen.findByText(/Please assign a user/i)).toBeInTheDocument()
  })

  it('submits the form successfully for a new task', async () => {
    const user = userEvent.setup()
    let capturedPayload = null
    
    server.use(
      http.post('*/tasks', async ({ request }) => {
        capturedPayload = await request.json()
        return HttpResponse.json({ id: 999, ...capturedPayload }, { status: 201 })
      })
    )

    renderWithProviders(<TaskForm />)
    await screen.findByRole('heading', { name: /Create New Task/i })

    // Now linked correctly via htmlFor/id
    const titleInput = screen.getByLabelText(/Task Title/i)
    await user.type(titleInput, 'New Test Task')
    
    await screen.findByText('Bob Smith')
    await user.selectOptions(screen.getByLabelText(/Assign To/i), '1')
    await user.selectOptions(screen.getByLabelText(/Priority/i), 'High')
    
    // Fill in the optional but schema-validated Due Date
    const dateInput = screen.getByLabelText(/Due Date/i)
    await user.type(dateInput, '2027-12-31T23:59')

    await user.click(screen.getByRole('button', { name: /Create Task/i }))

    await waitFor(() => {
      if (!capturedPayload) throw new Error('Payload not captured')
    }, { timeout: 4000 })
    
    expect(capturedPayload).toMatchObject({
      title: 'New Test Task',
      userId: 1,
      priority: 'High'
    })
  })

  it('renders in edit mode and populates data', async () => {
    renderWithProviders(<TaskForm />, {
      route: '/edit-task/123',
      path: '/edit-task/:id'
    })

    expect(await screen.findByRole('heading', { name: /Editing Task/i }, { timeout: 3000 })).toBeInTheDocument()
    
    await waitFor(() => {
        // Use getByDisplayValue to verify form population
        expect(screen.getByDisplayValue('Existing Task')).toBeInTheDocument()
        expect(screen.getByDisplayValue('Old desc')).toBeInTheDocument()
    }, { timeout: 3000 })
  })
})
