import { CssBaseline } from "@mui/material";
import { Route, Routes, Navigate, useLocation } from "react-router-dom";
import { HomeView } from "./pages/homeView";
import { LoginView } from "./pages/loginView";
import { useService } from './services/useService';
import { useObserver } from "mobx-react-lite";
import {  ThemeProvider, createTheme } from '@mui/material/styles';
import './App.css';

const theme = createTheme();

function App() {
  const location = useLocation();
  const auth = useService().Authentification;
/*  if(!auth.logged && getPernamentSessionId()){
    auth.tryPernamentLogin()
  }*/
  return useObserver(() => {
    return (
      <>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <Routes>
          <Route path="/login" element={<LoginView />} />
          <Route
            path="/"
            element={auth.logged ? (<HomeView />) : (<Navigate to="/login" state={{ from: location }} replace />)}
          />
        </Routes>
        </ThemeProvider>
      </>
    )
  });
}

export default App;
