import { useState } from "react";
import React, { useRef, useEffect } from "react";
import { Link as RouterLink, useLocation, useNavigate } from "react-router-dom";
import { useService } from "../../core/useService"
import { observer } from "mobx-react-lite";

import {
    Container,
    Box,
    FormControl,
    TextField,
    InputAdornment,
    IconButton,
    Stack,
    FormControlLabel,
    Checkbox,
    Button
} from "@mui/material";

import styled from "@emotion/styled";
import { Icon } from "@iconify/react";

const RootStyle = styled("div")({
    background: "rgb(249, 250, 251)",
    height: "100vh",
    display: "grid",
    placeItems: "center",
});

const ContentStyle = styled("div")({
    maxWidth: 480,
    padding: 25,
    margin: "auto",
    display: "flex",
    justifyContent: "center",
    flexDirection: "column",
    background: "#fff",
});

export const LoginView = observer(() => {
    const auth = useService().Authentification;
    const [showPassword, setShowPassword] = useState(false);
    const [rememberMe, setRememberMe] = useState(false);
    const navigate = useNavigate();
    const location = useLocation();
    const from = location.state?.from?.pathname || "/";
    const userNameRef = useRef<HTMLInputElement>(null)
    const passwordRef = useRef<HTMLInputElement>(null)

    useEffect(() => {
        if (auth.logged || auth.logging) return;
        auth.tryPernamentLogin().then(redirect => {
            if (!redirect) return;
            const from = location.state?.from?.pathname || "/";
            navigate(from, { replace: true });
        })
    })

    async function handleSubmit(e: React.SyntheticEvent) {
        let userName = userNameRef.current?.value;
        let password = passwordRef.current?.value;
        if (!(userName && password)) return;
        var res = await auth.login(userName, password, rememberMe)
        if (res) {
            navigate(from, { replace: true });
        }
    }

    return (
        <RootStyle>
            <Container maxWidth="sm">
                <ContentStyle>
                    <FormControl disabled={auth.logging} >
                        <Box
                            sx={{
                                display: "flex",
                                flexDirection: "column",
                                gap: 3,
                            }}
                        >
                            <TextField
                                fullWidth
                                autoComplete="username"
                                type="email"
                                label="User name"
                                inputRef={userNameRef}
                                disabled={auth.logging}
                            />
                            <TextField
                                fullWidth
                                autoComplete="current-password"
                                type={showPassword ? "text" : "password"}
                                label="Password"
                                inputRef={passwordRef}
                                InputProps={{
                                    endAdornment: (
                                        <InputAdornment position="end">
                                            <IconButton onClick={() => setShowPassword((prev) => !prev)}>
                                                {showPassword ? (<Icon icon="eva:eye-fill" />) : (<Icon icon="eva:eye-off-fill" />)}
                                            </IconButton>
                                        </InputAdornment>
                                    )
                                }}
                                disabled={auth.logging}
                            />
                        </Box>
                        <Box>
                            <Stack
                                direction="row"
                                alignItems="center"
                                justifyContent="space-between"
                                sx={{ my: 2 }}
                            >
                                <FormControlLabel
                                    control={<Checkbox value={rememberMe} onClick={() => setRememberMe(!rememberMe)} />}
                                    label="Remember me"
                                />
                            </Stack>
                            <Box textAlign='center'><Button type="submit" variant="contained" onClick={handleSubmit}>Login</Button></Box>
                        </Box>
                    </FormControl>
                </ContentStyle>
            </Container>
        </RootStyle >
    );
});