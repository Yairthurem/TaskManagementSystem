import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'

export const apiSlice = createApi({
  reducerPath: 'api',
  // Localhost .NET Web Api default http port
  baseQuery: fetchBaseQuery({ baseUrl: 'http://localhost:5050/api/' }), 
  tagTypes: ['Task', 'User', 'Tag'],
  endpoints: (builder) => ({
    getUsers: builder.query({
      query: () => 'users',
      providesTags: ['User']
    }),
    createUser: builder.mutation({
      query: (user) => ({
        url: 'users',
        method: 'POST',
        body: user
      }),
      invalidatesTags: ['User']
    }),
    getTags: builder.query({
      query: () => 'tags',
      providesTags: ['Tag']
    }),
    createTag: builder.mutation({
      query: (tag) => ({
        url: 'tags',
        method: 'POST',
        body: tag
      }),
      invalidatesTags: ['Tag']
    }),
    getTasks: builder.query({
      query: () => 'tasks',
      providesTags: ['Task']
    }),
    createTask: builder.mutation({
      query: (task) => ({
        url: 'tasks',
        method: 'POST',
        body: task
      }),
      invalidatesTags: ['Task']
    }),
    updateTask: builder.mutation({
      query: ({ id, ...task }) => ({
        url: `tasks/${id}`,
        method: 'PUT',
        body: task
      }),
      invalidatesTags: ['Task']
    }),
    deleteTask: builder.mutation({
      query: (id) => ({
        url: `tasks/${id}`,
        method: 'DELETE'
      }),
      invalidatesTags: ['Task']
    })
  })
})

export const { 
    useGetUsersQuery, 
    useCreateUserMutation,
    useGetTagsQuery,
    useCreateTagMutation,
    useGetTasksQuery, 
    useCreateTaskMutation, 
    useUpdateTaskMutation,
    useDeleteTaskMutation 
} = apiSlice
