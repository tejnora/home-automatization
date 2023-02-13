import { useContext } from 'react';
import { ServicesContext } from "./serviceProvider"
import { ApplicationServices } from "./applicationServices"

export const useService = (): ApplicationServices => useContext(ServicesContext);