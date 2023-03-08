import { useState } from "react";
import {
    Container,
    Toolbar,
    Typography,
    Box,
    IconButton,
    Menu,
    Tooltip,
    Divider,
    List,
    ListItemButton,
    ListItemIcon,
    ListItemText,
    Grid,
    Paper
} from "@mui/material";
import { styled } from '@mui/material/styles';
import MuiDrawer from '@mui/material/Drawer';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import * as React from 'react';
import DashboardIcon from '@mui/icons-material/Dashboard';
import LayersIcon from '@mui/icons-material/Layers';
import PeopleIcon from '@mui/icons-material/People';
import MuiAppBar, { AppBarProps as MuiAppBarProps } from '@mui/material/AppBar';
import MenuItem from '@mui/material/MenuItem';
import MenuIcon from '@mui/icons-material/Menu';
import Avatar from '@mui/material/Avatar';
import { observer } from "mobx-react-lite";
import { useService } from "../services/useService"

const settingItems = ['Profile', 'Logout'];
const menuItems = [
    {
        name: "Dashboard",
        link: "Dashboard",
        icon: <DashboardIcon />
    },
    {
        name: "Doors",
        link: "Doors",
        icon: <LayersIcon />
    },
    {
        name: "Users",
        link: "Users",
        icon: <PeopleIcon />
    }];

const drawerWidth: number = 240;
const Drawer = styled(MuiDrawer, { shouldForwardProp: (prop) => prop !== 'open' })(
    ({ theme, open }) => ({
        '& .MuiDrawer-paper': {
            position: 'relative',
            whiteSpace: 'nowrap',
            width: drawerWidth,
            transition: theme.transitions.create('width', {
                easing: theme.transitions.easing.sharp,
                duration: theme.transitions.duration.enteringScreen,
            }),
            boxSizing: 'border-box',
            ...(!open && {
                overflowX: 'hidden',
                transition: theme.transitions.create('width', {
                    easing: theme.transitions.easing.sharp,
                    duration: theme.transitions.duration.leavingScreen,
                }),
                width: theme.spacing(7),
                [theme.breakpoints.up('sm')]: {
                    width: theme.spacing(9),
                },
            }),
        },
    }),
);

interface AppBarProps extends MuiAppBarProps {
    open?: boolean;
}

const AppBar = styled(MuiAppBar, {
    shouldForwardProp: (prop) => prop !== 'open',
})<AppBarProps>(({ theme, open }) => ({
    zIndex: theme.zIndex.drawer + 1,
    transition: theme.transitions.create(['width', 'margin'], {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.leavingScreen,
    }),
    ...(open && {
        marginLeft: drawerWidth,
        width: `calc(100% - ${drawerWidth}px)`,
        transition: theme.transitions.create(['width', 'margin'], {
            easing: theme.transitions.easing.sharp,
            duration: theme.transitions.duration.enteringScreen,
        }),
    }),
}));

function Copyright(props: any) {
    return (
        <Typography variant="body2" color="text.secondary" align="center" {...props}>
            {'Copyright © David Tejnora ' + new Date().getFullYear() + "."}
        </Typography>
    );
}

function MainMenu() {
    return (
        <React.Fragment>
            {
                menuItems.map(element => {
                    return (
                        <ListItemButton>
                            <ListItemIcon>
                                {element.icon}
                            </ListItemIcon>
                            <ListItemText primary={element.name} />
                        </ListItemButton>)
                })
            }
        </React.Fragment>
    )
}

function UserMenu() {
    const authentification = useService().Authentification;
    const [anchorElUser, setAnchorElUser] = useState<null | HTMLElement>(null);

    const handleOpenUserMenu = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorElUser(event.currentTarget);
    };

    const handleCloseUserMenu = (e: React.SyntheticEvent) => {
        setAnchorElUser(null);
    };

    function handleMenuClick(name: string) {
        if (name == "Logout") {
            authentification.logout();
        }
        setAnchorElUser(null);
    }

    return (
        <Box sx={{ flexGrow: 0 }}>
            <Tooltip title="Open settings">
                <IconButton onClick={handleOpenUserMenu} sx={{ p: 0 }}>
                    <Avatar />
                </IconButton>
            </Tooltip>
            <Menu
                sx={{ mt: '45px' }}
                id="menu-appbar"
                anchorEl={anchorElUser}
                anchorOrigin={{
                    vertical: 'top',
                    horizontal: 'right',
                }}
                keepMounted
                transformOrigin={{
                    vertical: 'top',
                    horizontal: 'right',
                }}
                open={Boolean(anchorElUser)}
                onClose={handleCloseUserMenu}
            >
                {settingItems.map((setting) => (
                    <MenuItem onClick={() => handleMenuClick(setting)}>
                        <Typography textAlign="center">{setting}</Typography>
                    </MenuItem>
                ))}
            </Menu>
        </Box>
    )
}

export const HomeView = observer(() => {
    const [open, setOpen] = useState(true);
    const toggleDrawer = () => { setOpen(!open); };

    return (
        <Box sx={{ display: 'flex' }}>
            <AppBar position="absolute" open={open}>
                <Toolbar
                    sx={{
                        pr: '24px'
                    }}
                >
                    <IconButton
                        edge="start"
                        color="inherit"
                        aria-label="open drawer"
                        onClick={toggleDrawer}
                        sx={{
                            marginRight: '36px',
                            ...(open && { display: 'none' }),
                        }}
                    >
                        <MenuIcon />
                    </IconButton>
                    <Typography
                        component="h1"
                        variant="h6"
                        color="inherit"
                        noWrap
                        sx={{ flexGrow: 1 }}
                    >
                        Dashboard
                    </Typography>
                    <UserMenu />
                </Toolbar>
            </AppBar>
            <Drawer variant="permanent" open={open}>
                <Toolbar
                    sx={{
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'flex-end',
                        px: [1],
                    }}
                >
                    <IconButton onClick={toggleDrawer}>
                        <ChevronLeftIcon />
                    </IconButton>
                </Toolbar>
                <Divider />
                <List component="nav"><MainMenu /></List>
            </Drawer>
            <Box
                component="main"
                sx={{
                    backgroundColor: (theme) =>
                        theme.palette.mode === 'light'
                            ? theme.palette.grey[100]
                            : theme.palette.grey[900],
                    flexGrow: 1,
                    height: '100vh',
                    overflow: 'auto',
                }}
            >
                <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
                    <Grid container spacing={3}>
                        <Grid item xs={12} md={8} lg={9}>
                            <Paper
                                sx={{
                                    p: 2,
                                    display: 'flex',
                                    flexDirection: 'column',
                                    height: 240,
                                }}
                            >
                            </Paper>
                        </Grid>
                    </Grid>
                    <Copyright sx={{ pt: 4 }} />
                </Container>
            </Box>
        </Box>
    );
});
export default HomeView;