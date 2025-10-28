import React from 'react'
import { Card, CardHeader, CardPreview, Text, makeStyles, shorthands, Caption1, Body1Strong } from '@fluentui/react-components'
import LevelBadge from '../Badge/LevelBadge.jsx'
import { levelToLabel, specialistToLabel } from '../../../utils/enums'

const useStyles = makeStyles({
    card: { height: '100%', display: 'grid', alignContent: 'start', ...shorthands.gap('8px') },
    flow: { display: 'grid', rowGap: '6px' },
    flowItem: { display: 'grid', gridTemplateColumns: '92px 1fr', columnGap: '8px', alignItems: 'center' },
    tag: { fontSize: 12, padding: '2px 8px', borderRadius: 8, backgroundColor: '#111', color: '#fff', width: 'fit-content' },
    code: { fontSize: 12, background: '#f6f6f6', padding: '4px 6px', borderRadius: 6, overflowX: 'auto' }
})

export default function CaseCard({ data }) {
    const s = useStyles()
    return (
        <Card className={s.card} appearance="filled">
            <CardHeader
                header={<Body1Strong>{data.name}</Body1Strong>}
                description={<Caption1>{new Date(data.createdAt).toLocaleString()}</Caption1>}
                action={<LevelBadge level={data.level} />}
            />
            <CardPreview style={{ margin: 0 }}>
                <div style={{ display: 'grid', gap: 6 }}>
                    <Text wrap>{data.symptoms}</Text>
                    <Text><strong>Severity:</strong> {levelToLabel(data.level)}</Text>
                    <Text><strong>Specialist:</strong> {specialistToLabel(data.specialist)}</Text>
                    <Text><strong>First Aid:</strong> {data.firstAid || 'Pending'}</Text>
                    <Text><strong>Advice:</strong> {data.advice || 'Pending'}</Text>
                    <Text><strong>Status:</strong> {data.status}</Text>
                </div>
            </CardPreview>
            <div className={s.flow}>
                {(data.steps || []).map((s, i) => (
                    <div key={i} className={s.flowItem}>
                        <span className={s.tag}>{s.step}</span>
                        <code className={s.code}>{JSON.stringify(s.payload)}</code>
                    </div>
                ))}
            </div>
        </Card>
    )
}
