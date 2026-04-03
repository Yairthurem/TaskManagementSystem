import { render, screen } from '@testing-library/react'
import { describe, it, expect } from 'vitest'
import { Provider } from 'react-redux'
import { store } from './store/store'
import App from './App'

describe('App React Ecosystem', () => {
  it('renders the initial layout wrapper beautifully', () => {
    
    // We physically mount the Application heavily surrounded by the real Redux Provider to prove store integration
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
