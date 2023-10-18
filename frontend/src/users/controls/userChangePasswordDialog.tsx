import React, { useState, useRef } from "react";
import {
    Button,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField
} from "@mui/material";
import { usersClient } from "../services/usersApi"
import { useService } from "../../core/useService"

interface ChangePasswordDialogProps {
    openDialog: boolean;
    setOpenDialog: React.Dispatch<React.SetStateAction<boolean>>;
    userName: string;
}
export default function ChangePasswordDialog(props: ChangePasswordDialogProps) {
    const newPasswordRef = useRef<HTMLInputElement>(null)
    const originPasswordRef = useRef<HTMLInputElement>(null)

    const handleClose = () => {
        props.setOpenDialog(false);
    }

    async function handleChange() {
        props.setOpenDialog(false);
        const newPass = newPasswordRef.current?.value;
        const originPass = originPasswordRef.current?.value;
        if (!newPass || !originPass) return;
        usersClient.userChangePassword(props.userName, newPass, originPass);
    }

    return (
        <Dialog open={props.openDialog} onClose={handleClose}>
            <DialogTitle>Change Password</DialogTitle>
            <DialogContent>
                <TextField
                    autoFocus
                    margin="dense"
                    id="name"
                    label="New Password"
                    type="password"
                    fullWidth
                    variant="standard"
                    inputRef={newPasswordRef}
                />
                <TextField
                    autoFocus
                    margin="dense"
                    id="name"
                    label="Origin Password"
                    type="password"
                    fullWidth
                    variant="standard"
                    inputRef={originPasswordRef}
                />
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose}>Cancel</Button>
                <Button onClick={handleChange}>Change</Button>
            </DialogActions>
        </Dialog>
    );
}