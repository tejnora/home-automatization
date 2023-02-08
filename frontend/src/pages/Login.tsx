import { useState } from "react";
import React from "react";
import { Link as RouterLink } from "react-router-dom";

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

type LoginProps = {
    setAuth: React.Dispatch<React.SetStateAction<boolean>>;
};

const Login = ({ setAuth }: LoginProps) => {
    const [showPassword, setShowPassword] = useState(false);
    
    function handleSubmit(e: React.SyntheticEvent){
        console.log("submit");
        setAuth(true);
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
                                label="Email Address"
                            />
                            <TextField
                                fullWidth
                                autoComplete="current-password"
                                type={showPassword ? "text" : "password"}
                                label="Password"
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
                                    control={<Checkbox/>}
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
                            <Box textAlign='center'><Button type="submit" variant="contained" onClick={handleSubmit}>"Login"</Button></Box>
                        </Box>
                    </FormControl>
                </ContentStyle>
            </Container>
        </RootStyle >
    );
};
export default Login;