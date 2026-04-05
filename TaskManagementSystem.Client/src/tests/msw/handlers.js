import { http, HttpResponse } from 'msw'

export const handlers = [
  // Users
  http.get('*/users', () => {
    return HttpResponse.json([
      { id: 1, firstName: 'Bob', lastName: 'Smith' }
    ])
  }),

  // Tags
  http.get('*/tags', () => {
    return HttpResponse.json([
      { id: 1, name: 'Work' },
      { id: 2, name: 'Urgent' }
    ])
  }),

  // Tasks
  http.get('*/tasks', () => {
    return HttpResponse.json([
      { 
        id: 123, 
        title: 'Existing Task', 
        description: 'Old desc', 
        dueDate: '2025-12-21T12:00',
        priority: 'High', 
        status: 'ToDo',
        userId: 1, 
        tagIds: [1], 
        user: { firstName: 'John' }, 
        tags: ['Work'] 
      }
    ])
  }),

  http.post('*/tasks', async ({ request }) => {
    const newTask = await request.json()
    return HttpResponse.json({ id: 999, ...newTask }, { status: 201 })
  }),

  http.put('*/tasks/:id', async ({ request, params }) => {
    const updatedTask = await request.json()
    return HttpResponse.json({ id: parseInt(params.id), ...updatedTask })
  }),

  http.delete('*/tasks/:id', () => {
    return new HttpResponse(null, { status: 204 })
  }),
]
