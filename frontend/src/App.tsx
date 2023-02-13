import { useState } from 'react';
import { CssBaseline } from "@mui/material";
import { Route, Routes, Navigate, useLocation } from "react-router-dom";
import { HomeView } from "./pages/homeView";
import { LoginView } from "./pages/loginView";
import { useService } from './services/useService';
import { useObserver } from "mobx-react-lite";
import './App.css';

function App() {
  const location = useLocation();
  const auth = useService().Authentification;
  return useObserver(() => {
    return (
      <>
        <CssBaseline />
        <Routes>
          <Route path="/login" element={<LoginView />} />
          <Route
            path="/"
            element={auth.logged ? (<HomeView />) : (<Navigate to="/login" state={{ from: location }} replace />)}
          />
        </Routes>
      </>
    )
  });
}

export default App;
