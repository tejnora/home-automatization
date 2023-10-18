declare global {  
    interface String {  
        toDateTime(): Date;  
    }  
}  
String.prototype.toDateTime = function(): Date {  
    var intStringValue = this.replace(/\/+Date\(([\d+-]+)\)\/+/, '$1');
    var intValue = parseInt(intStringValue, 10);
    return new Date(intValue);
}  
export {}; 