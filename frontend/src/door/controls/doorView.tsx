import { observer } from "mobx-react-lite";
import {
    Paper,
    CssBaseline,
    Button
} from "@mui/material";
import Title from "../../gui/title"
import { doorClient } from "../services/DoorApi"

export const DoorView = observer(() => {
    async function handleOpenDoor(e: React.SyntheticEvent) {
        doorClient.openDoor();
    }
    return (
        <div>
            <CssBaseline />
            <Paper sx={{
                p: 2,
                display: 'flex',
                flexDirection: 'column',
                height: "calc(100vh - 200px)",
            }}
            >
                <Title>Doors</Title>
               <Button type="submit" variant="contained" onClick={handleOpenDoor}>Open door</Button>
            </Paper>
        </div>
    )
});
