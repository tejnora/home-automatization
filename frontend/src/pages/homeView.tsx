import { Button, Typography, Container, Box } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { observer } from "mobx-react-lite";
import { useService } from "../services/useService"

export const HomeView = observer(() => {
    const authentification = useService().Authentification;
    const theme = useTheme();

    function handleLogout(e: React.SyntheticEvent) {
        authentification.logout();
        console.info("logout");
    }

    return (
        <Container
            maxWidth="lg"
            sx={{
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                flexDirection: "column",
                height: "100vh",
            }}
        >
            <Typography
                sx={{
                    textAlign: "center",
                    marginTop: "-4rem",
                    fontSize: "5rem",
                    fontWeight: 700,
                    letterSpacing: "-0.5rem",
                    display: "inline-block",
                    whiteSpace: "nowrap",
                    [theme.breakpoints.down("sm")]: {
                        fontSize: "4rem",
                        letterSpacing: "-0.4rem",
                    },
                }}
                gutterBottom
            >
                Welcome Back
            </Typography>
            <Button size="large" variant="contained" onClick={handleLogout}>
                Log out
            </Button>
        </Container>
    );
});
export default HomeView;