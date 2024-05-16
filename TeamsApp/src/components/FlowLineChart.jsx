import React from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

const FlowLineChart = ({ flows }) => {
    if (!flows || flows.length === 0) {
        return <div>No data available</div>;
    }

    const lineColors = [
        "#8884d8",
        "#82ca9d",
        "#ffc658",
        "#ff7300",
        "#d0ed57",
        "#a4de6c",
    ];

    return (
        <ResponsiveContainer width="100%" height="100%">
            <LineChart>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="time" type="category" allowDuplicatedCategory={false} />
                <YAxis dataKey="statecode" domain={[-0.1, 1.1]} ticks={[0, 1]} />
                <Tooltip />
                <Legend />
                {flows.map((s, index) => (
                    <Line
                        key={s.name}
                        type="monotone"
                        dataKey="statecode"
                        data={s.data || []}
                        name={s.name}
                        stroke={lineColors[index % lineColors.length]}
                    />
                ))}
            </LineChart>
        </ResponsiveContainer>
    );
}

export default FlowLineChart;