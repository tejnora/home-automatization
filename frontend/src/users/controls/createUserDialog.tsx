import React, { useRef } from "react";
import {
    Button,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField
} from "@mui/material";

interface CreateUserDialogProps {
    openDialog: boolean;
    setOpenDialog: React.Dispatch<React.SetStateAction<boolean>>;
    createUser(user: string, password: string): void;
}
export default function CreateUserDialog(props: CreateUserDialogProps) {
    const passwordRef = useRef<HTMLInputElement>(null)
    const userRef = useRef<HTMLInputElement>(null)

    const handleClose = () => {
        props.setOpenDialog(false);
    }

    function handleCreate() {
        props.setOpenDialog(false);
        const newPass = passwordRef.current?.value;
        const user = userRef.current?.value;
        if (newPass === undefined || user === undefined) return;
        props.createUser(user, newPass);
    }

    return (
        <Dialog open={props.openDialog} onClose={handleClose}>
            <DialogTitle>Create User</DialogTitle>
            <DialogContent>
                <TextField
                    autoComplete="off"
                    autoFocus
                    margin="dense"
                    label="User name"
                    type='text'
                    fullWidth
                    variant="standard"
                    inputRef={userRef}
                />
                <TextField
                    autoComplete="off"
                    margin="dense"
                    label="New Password"
                    type="password"
                    fullWidth
                    variant="standard"
                    inputRef={passwordRef}
                />
            </DialogContent>
            <DialogActions>
                <Button onClick={handleCreate}>Create</Button>
            </DialogActions>
        </Dialog>
    );
}