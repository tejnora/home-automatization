import { observer } from "mobx-react-lite";
import React, { useState, Suspense, lazy } from 'react'
import { suspend } from 'suspend-react'
import { imagegalleryClient, IImageGroupsResponse, IImagesListResponse, IImageInfo } from "../services/imagegalleryApi"
import Title from "../../gui/title"
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import ListItemAvatar from '@mui/material/ListItemAvatar';
import Avatar from '@mui/material/Avatar';
import FolderIcon from '@mui/icons-material/Folder';
import ImageList from '@mui/material/ImageList';
import ImageListItemBar from "@mui/material/ImageListItemBar";
import Backdrop from "@mui/material/Backdrop"
import Button from "@mui/material/Button"
import Box from "@mui/material/Box";
import { createTheme, ThemeProvider } from "@mui/material/styles";
import ImageListItem, {
    imageListItemClasses
} from "@mui/material/ImageListItem";
import IconButton from "@mui/material/IconButton";
import InfoIcon from "@mui/icons-material/Info";
import { colors, useMediaQuery } from "@mui/material";

interface IGroupsListProps {
    selectGroup(name: string): void;
}

const GroupsList = React.lazy(() => imagegalleryClient.listOfImageGroups().then((groups: IImageGroupsResponse) => {
    return {
        default: (props: IGroupsListProps) => {
            const listItems = groups.ImagesGroups.map((group, index) => (
                <ListItem key={index}>
                    <ListItemAvatar>
                        <Avatar>
                            <FolderIcon />
                        </Avatar>
                    </ListItemAvatar>
                    <ListItemText primary={group} onClick={() => { props.selectGroup(group); }} key={group} />
                </ListItem>
            ));
            return (
                <div>
                    <Title>"Images Groups"</Title>
                    <List sx={{ width: '100%', maxWidth: 360, bgcolor: 'background.paper' }}>
                        {listItems}
                    </List>
                </div>)
        }
    }
}));

const theme = createTheme({
    breakpoints: {
        values: {
            xs: 0,
            sm: 350,
            md: 650,
            lg: 900,
            xl: 1200,
        },
    },
});

interface IImagesListProps {
    imageGroup: string;
    selectGroup(name: string): void;
}

function ImagesList(props: IImagesListProps) {
    const [imagesList, setImagesList] = useState<IImageInfo[]>([]);
    const [load, setLoad] = useState(true);
    if (load === true) {
        setLoad(false);
        imagegalleryClient.imagesList(props.imageGroup).then((images: IImagesListResponse) => {
            setImagesList(images.Images);
        });
    }
    const [backdropImage, setBackdropImage] = useState("");
    const matches = useMediaQuery('(min-width:600px)');
    /*    <Box
        sx={{
          gap: 1,
          height: 450,
          display: "grid",
          gridTemplateColumns: {
            xs: "repeat(1, 1fr)",
            sm: "repeat(2, 1fr)",
            md: "repeat(3, 1fr)",
            lg: "repeat(4, 1fr)",
            xl: "repeat(4, 1fr)"
          },
          [`& .${imageListItemClasses.root}`]: {
            display: "flex",
            flexDirection: "column"
          }
        }}
      >
    */
    return (
        <ThemeProvider theme={theme}>
            <Button variant="contained" onClick={()=>{props.selectGroup("")}}>Close {props.imageGroup}</Button>
            <ImageList cols={matches ? 4 : 3} variant="masonry">
                {imagesList.map((item) => (
                    <ImageListItem key={item.Src}>
                        <img
                            srcSet={`img/${props.imageGroup}/${item.Src}?w=128&h=128&fit=crop&auto=format&dpr=2 2x`}
                            src={`img/${props.imageGroup}/${item.Src}?w=128&h=128&fit=crop&auto=format`}
                            alt={item.Name}
                            loading="lazy"
                            onClick={() => { setBackdropImage(item.Src) }}
                        />
                        <ImageListItemBar
                            title={item.Name}
                            position="below"
                            actionIcon={
                                <IconButton
                                    sx={{ color: "rgba(0, 0, 255, 0.54)" }}
                                    aria-label={`info about ${item}`}
                                >
                                    <InfoIcon />
                                </IconButton>
                            }
                        />
                    </ImageListItem>
                ))}
            </ImageList>
            <Backdrop
                sx={(theme) => ({ color: '#fff', zIndex: theme.zIndex.drawer + 1 })}
                open={backdropImage !== ""}
                onClick={() => { setBackdropImage("") }}
            >
                <img src={`img/${props.imageGroup}/${backdropImage}`} />
            </Backdrop>
        </ThemeProvider>
    );
}

export const ImageGalleryView = observer(() => {
    const [selectedGroup, setselectedGroup] = useState("");
    return (
        <div>
            <Suspense fallback={"Loading..."}>
                {selectedGroup === "" ? <GroupsList selectGroup={setselectedGroup} /> : <ImagesList imageGroup={selectedGroup} selectGroup={setselectedGroup}/>}
            </Suspense>
        </div>
    )
});
