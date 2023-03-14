import { useState } from "react";
import React, { useRef } from "react";
import { Link as RouterLink, useLocation, useNavigate } from "react-router-dom";
import { useService } from "../../core/useService"
import { observer } from "mobx-react-lite";

import {
    Container,
    Link,
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
    const authentification = useService().Authentification;
    const [showPassword, setShowPassword] = useState(false);
    const navigate = useNavigate();
    const location = useLocation();
    const from = location.state?.from?.pathname || "/";
    const userNameRef = useRef<HTMLInputElement>(null)
    const passwordRef = useRef<HTMLInputElement>(null)

    async function handleSubmit(e: React.SyntheticEvent) {
        let userName = userNameRef.current?.value;
        let password = passwordRef.current?.value;
        if(!(userName && password))return;
        var res = await authentification.login(userName, password)
        if (res) {
            navigate(from, { replace: true });
        }
    }
    return (
        <RootStyle>
            <Container maxWidth="sm">
                <ContentStyle>
                    <FormControl>
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
                                    ),
                                }}
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
                                    control={<Checkbox />}
                                    label="Remember me"
                                />
                                <Link
                                    component={RouterLink}
                                    variant="subtitle2"
                                    to="#"
                                    underline="hover"
                                >
                                    Forgot password?
                                </Link>
                            </Stack>
                            <Box textAlign='center'><Button type="submit" variant="contained" onClick={handleSubmit}>Login</Button></Box>
                        </Box>
                    </FormControl>
                </ContentStyle>
            </Container>
        </RootStyle >
    );
});