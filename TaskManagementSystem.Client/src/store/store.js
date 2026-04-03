import { configureStore, isRejectedWithValue } from '@reduxjs/toolkit'
import { apiSlice } from './apiSlice'
import { toast } from 'react-toastify'

export const rtkQueryErrorLogger = (api) => (next) => (action) => {
  if (isRejectedWithValue(action)) {
    const errorData = action.payload?.data;
    if (errorData?.Message) {
        toast.error(errorData.Message);
    } else {
        toast.error('An unexpected server error occurred.');
    }
  }
  return next(action)
}

export const store = configureStore({
  reducer: {
    [apiSlice.reducerPath]: apiSlice.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(apiSlice.middleware, rtkQueryErrorLogger),
})
