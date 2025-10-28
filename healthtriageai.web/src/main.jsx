import { createRoot } from 'react-dom/client'
import { FluentProvider, teamsDarkTheme } from '@fluentui/react-components';
import './index.css'
import App from './App.jsx'

createRoot(document.getElementById('root')).render(
    <FluentProvider theme={teamsDarkTheme}>
        <App />
    </FluentProvider>,
)