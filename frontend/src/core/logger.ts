 class LoggerImpl {

    logServiceError(message: any) {
        const timestamp: string = new Date().toISOString()
        console.log(`${timestamp} - ${message}`)
    }
};

export const Logger = new LoggerImpl();