import { useEffect, useState } from 'react'
import './App.css'

function App() {
  const [message, setMessage] = useState('')

  useEffect(() => {
    fetch('/api/home')
        .then((res) => res.json())
        .then((data) => setMessage(data.message))
  }, [])

  return (
      <div>
        <p>{message}</p>
      </div>
  ) 
}

export default App