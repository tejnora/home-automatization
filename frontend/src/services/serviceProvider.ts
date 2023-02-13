import { createContext } from 'react';
import { ApplicationServices } from "./applicationServices"

export const ServicesContext = createContext<ApplicationServices>({} as ApplicationServices);
export const ServicesProvider = ServicesContext.Provider;