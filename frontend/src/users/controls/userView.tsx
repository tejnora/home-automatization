import { observer } from "mobx-react-lite";
import { Paper, CssBaseline } from "@mui/material";
import { DataGrid, GridColDef, GridValueGetterParams } from '@mui/x-data-grid';
import Title from "../../gui/title"

const columns: GridColDef[] = [
    { field: 'firstName', headerName: 'First name', width: 130},
    { field: 'lastName', headerName: 'Last name', width: 130 },
];

const rows = [
    { id: 1, lastName: 'Snow', firstName: 'Jon' },
    { id: 2, lastName: 'Lannister', firstName: 'Cersei' },
    { id: 3, lastName: 'Lannister', firstName: 'Jaime' },
    { id: 4, lastName: 'Stark', firstName: 'Arya' },
    { id: 5, lastName: 'Targaryen', firstName: 'Daenerys' },
];

export const UserView = observer(() => {
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
                <Title>Users</Title>
                <DataGrid rows={rows} columns={columns} />
            </Paper>
        </div>
    )
});