/*! Vera VAF 1.0 license: https://github.com/sparkledb/vera */

/**
* JSON escapes all unicode characters in the input string
* @param {String} s Input string to escape 
*/
function JsonEscapeUnicode(s)
{
    return s.replace(/[\u007f-\uffff]/g,
        function(c) { 
            return '\\u'+('0000'+c.charCodeAt(0).toString(16)).slice(-4);
        }
    );
}

/**
* Converts a object to a JSON string
* @param {Object} s Object to serialize
* @param {Boolean} emit_unicode Set to false to encode all unicode characters
*/
function JSON_stringify(s, emit_unicode)
{
    var json = JSON.stringify(s);
    return emit_unicode ? json : EncodeUnicode(json);
}

/**
* Creates a JSON representation of a VeraWAF.WebPages.Dal.Interchange.GenericResult data structure meant for 
* data interchange using the Vera REST APIs
* @param {String} tableName Azure table name. Ex. "VeraUsers"
* @param {String} entityType Entity type name. Ex. "UserEntity"
* @param {String} partitionKey Azure table partition key
* @param {String} propertyName Azure table row property name. Ex. "Username"
* @param {String} propertyValue Azure table row property value. Ex. "Jill"
*/
function Create_TablePropertyInfo(tableName, entityType, partitionKey, propertyName, propertyValue)
{
    var jsonData = {
        TableName: tableName,
        EntityType: entityType,
        PartitionKey: partitionKey, 
        PropertyName: propertyName, 
        PropertyValue: propertyValue
    };

    return JSON_stringify(jsonData, true);
}
