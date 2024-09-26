import { observer } from "mobx-react-lite";
import React, { useState, Suspense, lazy } from 'react'
import { suspend } from 'suspend-react'
import { imagegalleryClient, IImageGroupsResponse, IImagesListResponse } from "../services/imagegalleryApi"
import {
    Paper,
    CssBaseline,
} from "@mui/material";
import Title from "../../gui/title"
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import ListItemAvatar from '@mui/material/ListItemAvatar';
import Avatar from '@mui/material/Avatar';
import FolderIcon from '@mui/icons-material/Folder';
import ImageList from '@mui/material/ImageList';
import ImageListItem from '@mui/material/ImageListItem';

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

interface IImagesListProps {
    imageGroup: string;
}

const ImagesList1 = function (props: IImagesListProps) {
    const [imagesList, setImagesList] = useState([""]);
    imagegalleryClient.imagesList(props.imageGroup).then((images: IImagesListResponse) => {
        setImagesList(images.Images);
    });
    return (
        <ImageList sx={{ width: 500, height: 450 }} cols={3} rowHeight={164}>
            {imagesList.map((item) => (
                <ImageListItem key={item}>
                    <img
                        srcSet={`${item}?w=164&h=164&fit=crop&auto=format&dpr=2 2x`}
                        src={`${item}?w=164&h=164&fit=crop&auto=format`}
                        alt={item}
                        loading="lazy"
                    />
                </ImageListItem>
            ))}
        </ImageList>)
}


const ImagesList = React.lazy(() => imagegalleryClient.imagesList().then((images: IImagesListResponse) => {
    return {
        default: () => {
            return (<Title>"Images Groups"</Title>)
        }
    }
}));

export const ImageGalleryView = observer(() => {
    const [selectedGroup, setselectedGroup] = useState("");
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
                <Suspense fallback={"Loading..."}>
                    {selectedGroup === "" ? <GroupsList selectGroup={setselectedGroup} /> : <ImagesList />}
                </Suspense>
            </Paper>
        </div>
    )
});
