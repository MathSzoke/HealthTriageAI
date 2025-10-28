import React from 'react'
import { Badge } from '@fluentui/react-components'
import { levelToKey, levelToLabel } from '../../../utils/enums'

export default function LevelBadge({ level }) {
    const key = levelToKey(level)
    const label = levelToLabel(level)
    if (key === 'low') return <Badge appearance="tint" color="success">{label}</Badge>
    if (key === 'moderate') return <Badge appearance="tint" color="brand">{label}</Badge>
    if (key === 'high') return <Badge appearance="tint" color="warning">{label}</Badge>
    if (key === 'critical') return <Badge appearance="filled" color="danger">{label}</Badge>
    return <Badge>{label}</Badge>
}
