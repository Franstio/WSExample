import { useEffect, useState } from 'react'
import './App.css'
import './index.css'
import useWebSocket,{ReadyState} from 'react-use-websocket';
function App() {
  const [output,setOutput] = useState(0);
  const { sendJsonMessage, lastMessage, readyState } = useWebSocket("ws://localhost:5263/ws");

  const connectionStatus = {
    [ReadyState.CONNECTING]: 'Connecting',
    [ReadyState.OPEN]: 'Open',
    [ReadyState.CLOSING]: 'Closing',
    [ReadyState.CLOSED]: 'Closed',
    [ReadyState.UNINSTANTIATED]: 'Uninstantiated',
  }[readyState];

  useEffect(()=>{
      if (lastMessage == null)
        return;
      sendJsonMessage({msg:"start"});
      let payload : {output: number} = JSON.parse(lastMessage.data);
      setOutput(payload.output);
  },[lastMessage]);

  return (
    <>
      <div className='bg-cyan-400 p-12 rounded shadow-md '>
        <h1 className='text-3xl text-gray-800 font-bold'>Output: {output}</h1>
      </div>
    </>
  )
}

export default App
