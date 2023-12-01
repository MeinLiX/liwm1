export class EnumHelper {
    public static enumToString(enumObject: any, enumValue: number): string | undefined {
        const keys = Object.keys(enumObject).filter(key => enumObject[key] === enumValue);
        return keys.length > 0 ? keys[0] : undefined;
    }
}