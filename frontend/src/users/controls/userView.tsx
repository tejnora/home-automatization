import React, { useState, Suspense } from 'react'
import { Paper, CssBaseline, Checkbox } from "@mui/material";
import {
  DataGrid,
  GridColDef,
  GridActionsCellItem,
  GridRowId,
  GridToolbarContainer,
  GridRowsProp,
  GridRowModesModel,
  GridRowModes,
  GridRowParams,
  MuiEvent,
  GridEventListener,
  GridRowModel,
  GridRenderEditCellParams,
  GridValidRowModel
} from '@mui/x-data-grid';
import Title from "../../gui/title"
import Button from '@mui/material/Button';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/DeleteOutlined';
import SaveIcon from '@mui/icons-material/Save';
import CancelIcon from '@mui/icons-material/Close';
import ChnagePassworIcon from '@mui/icons-material/Password';
import { usersClient, IUserListResponse } from "../services/usersApi"
import { suspend } from 'suspend-react'
import CreateUserDialog from './createUserDialog';
import ChangePasswordDialog from './userChangePasswordDialog'

let idCounter = 0;

interface EditToolbarProps {
  setRows: (newRows: (oldRows: GridRowsProp) => GridRowsProp) => void;
  setRowModesModel: (newModel: (oldModel: GridRowModesModel) => GridRowModesModel) => void;
}

interface IRowData extends GridValidRowModel {
  id: number;
  name: string;
  password?: string;
  lastLogin: Date;
  enabled: boolean;
}

interface DataControlProps {
  updatePassword(name: string): void;
}

function EditToolbar(props: EditToolbarProps) {
  const { setRows, setRowModesModel } = props;
  const [openCreateUserDialog, setOpenCreateUserDialog] = useState(false);
  const handleClick = () => {
    setOpenCreateUserDialog(true);
  };
  return (
    <GridToolbarContainer>
      <CreateUserDialog openDialog={openCreateUserDialog} setOpenDialog={setOpenCreateUserDialog} createUser={
        (user: string, password: string) => {
          const id = idCounter++;
          setRows((oldRows) => [...oldRows, { id, name: user, password: password, isNew: true }]);
          setRowModesModel((oldModel) => ({
            ...oldModel,
            [id]: { mode: GridRowModes.Edit, fieldToFocus: 'name' },
          }));
        }} />
      <Button color="primary" startIcon={<AddIcon />} onClick={handleClick}>
        Add user
      </Button>
    </GridToolbarContainer>
  );
}

const DataControlLazy = (props: DataControlProps) => {
  const [rows, setRows] = useState<GridRowsProp<IRowData>>([]);
  const [rowModesModel, setRowModesModel] = React.useState<GridRowModesModel>({});
  const [load, setLoad] = useState(true);

  if (load === true) {
    setLoad(false);
    const data = suspend(async () => {
      const res = await usersClient.usersList();
      return res.Users
    }, ['usersList']);
    setRows(data.map<IRowData>((item) => {
      return {
        id: idCounter++,
        name: item.Name,
        lastLogin: item.LastLogin,
        enabled: item.Enabled
      }
    }));
  }

  const handleRowEditStart = (params: GridRowParams, event: MuiEvent<React.SyntheticEvent>,) => {
    event.defaultMuiPrevented = true;
  };

  const handleRowEditStop: GridEventListener<'rowEditStop'> = (params, event) => {
    event.defaultMuiPrevented = true;
  };

  const handleEditClick = (id: GridRowId) => () => {
    setRowModesModel({ ...rowModesModel, [id]: { mode: GridRowModes.Edit } });
  };

  const handleSaveClick = (id: GridRowId) => () => {
    setRowModesModel({ ...rowModesModel, [id]: { mode: GridRowModes.View } });
  };

  const handleDeleteClick = (id: GridRowId) => () => {
    const rowItem = rows.find((row) => row.id === id);
    if (rowItem === undefined) return;
    usersClient.removeUser(rowItem.name);
    setRows(rows.filter((row) => row.id !== id));
  };

  const handleChnagePassword = (id: GridRowId) => () => {
    const rowItem = rows.find((row) => row.id === id);
    if (rowItem === undefined) return;
    props.updatePassword(rowItem.name);
  }

  const handleCancelClick = (id: GridRowId) => () => {
    setRowModesModel({ ...rowModesModel, [id]: { mode: GridRowModes.View, ignoreModifications: true }, });
    const editedRow = rows.find((row) => row.id === id);
    if (editedRow!.isNew) {
      setRows(rows.filter((row) => row.id !== id));
    }
  };

  const processRowUpdate = (newRow: IRowData) => {
    const updatedRow = { ...newRow, isNew: false };
    setRows(rows.map((row) => (row.id === newRow.id ? updatedRow : row)));
    if (newRow.isNew) {
      usersClient.createUser(newRow.name, newRow.password as string, newRow.enabled);
    }
    else {
      usersClient.updateUser(newRow.name, newRow.enabled);
    }
    return updatedRow;
  };

  const handleRowModesModelChange = (newRowModesModel: GridRowModesModel) => {
    setRowModesModel(newRowModesModel);
  };

  const columns: GridColDef[] = [
    {
      field: 'enabled',
      headerName: 'Enabled',
      width: 100,
      editable: true,
      renderCell: (params: GridRenderEditCellParams) => (
        <Checkbox checked={params.value} disabled />
      ),
      renderEditCell: (params: GridRenderEditCellParams) => (
        <Checkbox checked={params.value} onChange={
          (e, newValue) => {
            const { id, api, field } = params;
            api.setEditCellValue({ id, field, value: newValue });
          }
        } />
      ),
    },
    { field: 'name', headerName: 'Name', width: 250, editable: false },
    {
      field: 'lastLogin',
      headerName: 'Last Login',
      width: 220,
      editable: false,
      valueGetter: (params) => {
        if (params.row.lastLogin == null || params.row.lastLogin.getYear() === 70) {
          return "N/A"
        }
        return params.row.lastLogin?.toLocaleString([], { dateStyle: 'short' });
      }
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 120,
      cellClassName: 'actions',
      getActions: ({ id }) => {
        const isInEditMode = rowModesModel[id]?.mode === GridRowModes.Edit;

        if (isInEditMode) {
          return [
            <GridActionsCellItem
              icon={<SaveIcon />}
              label="Save"
              onClick={handleSaveClick(id)}
            />,
            <GridActionsCellItem
              icon={<CancelIcon />}
              label="Cancel"
              className="textPrimary"
              onClick={handleCancelClick(id)}
              color="inherit"
            />
          ];
        }

        return [
          <GridActionsCellItem
            icon={<EditIcon />}
            label="Edit"
            className="textPrimary"
            onClick={handleEditClick(id)}
            color="inherit"
          />,
          <GridActionsCellItem
            icon={<DeleteIcon />}
            label="Delete"
            onClick={handleDeleteClick(id)}
            color="inherit"
          />,
          <GridActionsCellItem
            icon={<ChnagePassworIcon />}
            label="ChangePassword"
            onClick={handleChnagePassword(id)}
            color="inherit"
          />
        ];
      },
    },
  ];
  return (
    <DataGrid
      rows={rows}
      columns={columns}
      editMode="row"
      rowModesModel={rowModesModel}
      onRowModesModelChange={handleRowModesModelChange}
      onRowEditStart={handleRowEditStart}
      onRowEditStop={handleRowEditStop}
      processRowUpdate={processRowUpdate}
      slots={{
        toolbar: EditToolbar,
      }}
      slotProps={{
        toolbar: { setRows, setRowModesModel },
      }} />
  )
}

export const UserView = () => {
  const [changePasswordDialog, setChangePasswordDialog] = useState(false);
  const [changePasswordUserName, setchangePasswordUserName] = useState("");

  const handleSaveClick = (userName: string) => {
    setChangePasswordDialog(true);
    setchangePasswordUserName(userName);
  };

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
        <ChangePasswordDialog openDialog={changePasswordDialog} setOpenDialog={setChangePasswordDialog} userName={changePasswordUserName} />
        <Suspense fallback={"Loading..."}>
          <DataControlLazy updatePassword={(n) => handleSaveClick(n)} />
        </Suspense>
      </Paper>
    </div>
  )
};