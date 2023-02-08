import { useState } from 'react';
import { CssBaseline } from "@mui/material";
import { Route, Routes, Navigate, useLocation } from "react-router-dom";
import './App.css';
import Home from "./pages/Home";
import Login from "./pages/Login";

function App() {
  const [auth, setAuth] = useState(false);
  const location = useLocation();

  return (
    <>
      <CssBaseline />
      <Routes>
        <Route path="/login" element={<Login setAuth={setAuth} />} />
        <Route
          path="/"
          element={auth ? (<Home setAuth={setAuth} />) : (<Navigate to="/login" state={{ from: location }} replace />)}
        />
      </Routes>
    </>
  );
}

export default App;
