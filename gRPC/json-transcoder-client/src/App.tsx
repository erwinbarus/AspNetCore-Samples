import { useState } from 'react'
import './App.css'

function App() {
  const [name, setName] = useState('');
  const [message, setMessage] = useState('');

  async function getData() {
    const url = "http://localhost:5212/v1/greeter/" + name;
    try {
      const response = await fetch(url, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        }
      });
      if (!response.ok) {
        throw new Error(`Response status: ${response.status}`);
      }

      const result = await response.json();
      console.log(result);
      setMessage(result.message);
    } catch (e) {
      console.error(e);
    }
  }

  return (
    <>
      <div className="card">
        <input 
          value={name}
          onChange={e => setName(e.target.value)} />
        <button className='btn' onClick={getData}>
          Submit
        </button>
      </div>
      <div className='card'>
        <h2 id='message'>{message}</h2>
      </div>
    </>
  )
}

export default App
