export interface ISession {
    idType: number,
    idRole: number,
    idCompany: number,
    expiresSession: Date,
    username: string,
    token: string,
    whiteLabelConfig: any,
    isAssociacao: boolean,
    isQa: boolean,
    version: string
}
