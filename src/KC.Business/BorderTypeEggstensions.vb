Imports System.Runtime.CompilerServices

Friend Module BorderTypeEggstensions
    Private ReadOnly _table As IReadOnlyDictionary(Of BorderType, BorderTypeDescriptor) =
        New Dictionary(Of BorderType, BorderTypeDescriptor) From
        {
            {BorderType.Door, New BorderTypeDescriptor(True)},
            {BorderType.Wall, New BorderTypeDescriptor(False)}
        }
    <Extension>
    Function ToDescriptor(borderType As BorderType) As BorderTypeDescriptor
        Return _table(borderType)
    End Function
End Module
